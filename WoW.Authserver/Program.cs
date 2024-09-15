using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using WoW.Authserver.DB;
using WoW.Authserver.DB.Model;
using WoW.Client.Shared;
using WoW.Client.Shared.Auth;
using WoW.Client.Shared.Client;
using WoW.Server.Shared;
using WoW.Server.Shared.Serializable;

namespace WoW.Authserver
{
    internal class Program
    {
        private NetManager _netManager;
        private EventBasedNetListener _netEventListener;
        private static NetPacketProcessor _netProcessor;

        public static List<Realmserver> Realmlist = new List<Realmserver>();

        public Program()
        {
            Console.Title = "Authserver";

            using (var ctx = new AuthContext())
                // creates the database with/applies migrations.
                // todo: doesn't seem to ensure the tables are created if they get deleted, or that they have data in them.
                ctx.Database.Migrate();

            Console.WriteLine("Deleting all sessions...");
            using (var ctx = new AuthContext())
                ctx.Accounts.Where(a => a.SessionId != string.Empty)
                    .ExecuteUpdate(setters => setters
                        .SetProperty(p => p.SessionId, default(string)));

            _netProcessor = new NetPacketProcessor();

            _netEventListener = new EventBasedNetListener();
            _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.NetworkReceiveEvent += (peer, reader, delivery) => _netProcessor.ReadAllPackets(reader, peer);

            _netProcessor.SubscribeReusable<RealmAuth_Registrar, NetPeer>((newAuthRegistration, peer) =>
            {
                var newRealm = new Realmserver(newAuthRegistration.Name, newAuthRegistration.Ip, newAuthRegistration.Port);
                Realmlist.Add(newRealm);

                Console.WriteLine($"'{newRealm.Name}' has registered with realmlist ({newRealm.Ip}:{newRealm.Port})");
            });

            _netProcessor.SubscribeReusable<ClientAuth_Logon, NetPeer>((newAuth, peer) =>
            {
                Console.WriteLine($"{newAuth.AccountName} is trying to log in...");

                using (var ctx = new AuthContext())
                {
                    AuthClient_LogonCode loginCode = new AuthClient_LogonCode();
                    string accountSessionId = "";
                    var account = ctx.Accounts.FirstOrDefault(a => a.Username.Equals(newAuth.AccountName.ToLower()));

                    if (account != null && account.SessionId == default(string))
                    {
                        Console.WriteLine($"Generating session for {newAuth.AccountName}...");
                        accountSessionId = Guid.NewGuid().ToString().Replace("-", "");
                        account.SessionId = accountSessionId;
                        loginCode.Code = LogonCode.Success;
                    }
                    else if (account == null)
                    {
                        loginCode.Code = LogonCode.NoRecord;
                        return;
                    }
                    else
                    {
                        loginCode.Code = LogonCode.AlreadyOnline;
                        return;
                    }

                    Send(peer, loginCode);

                    if (loginCode.Code == LogonCode.Success)
                    {
                        Send(peer, new AuthClient_Logon() { SessionId = account.SessionId });

                        // todo: send all realms in one packet.
                        foreach (Realmserver realm in Realmlist)
                        {
                            AuthClient_Realmserver realmserver = new AuthClient_Realmserver()
                            {
                                Name = realm.Name,
                                Ip = realm.Ip,
                                Port = realm.Port,
                            };
                            Send(peer, realmserver);
                        }
                    }

                    ctx.SaveChanges();
                }
            });

            _netProcessor.SubscribeReusable<RealmAuth_SessionVerification, NetPeer>((request, peer) =>
            {
                using (var ctx = new AuthContext())
                {
                    Account account = ctx.Accounts.FirstOrDefault(a => a.SessionId.Equals(request.SessionId.ToLower()));
                    if (account != null)
                    {
                        Console.WriteLine("Sending user verification to realm...");
                        SendSerializable(peer, new AuthRealm_SessionVerification()
                        {
                            User = new PlayerAccount()
                            {
                                Id = account.Id,
                                SessionId = account.SessionId,
                                Security = account.Security
                            }
                        });
                    }
                }
            });

            _netManager = new NetManager(_netEventListener);
            _netManager.Start(8070);

            while (true)
            {
                _netManager.PollEvents();
            }
        }

        private static void Send<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(peer, packet, delivery);

        private static void SendSerializable<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => _netProcessor.SendNetSerializable(peer, packet, delivery);

        static void Main(string[] args)
            => new Program();
    }
}
