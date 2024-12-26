using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Net;
using WoW.Authserver.DB;
using WoW.Authserver.DB.Model;
using WoW.Client.Shared;
using WoW.Client.Shared.Auth;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Data;
using WoW.Server.Shared;
using WoW.Server.Shared.Serializable;
using static WoW.Server.Shared.Vocab;

namespace WoW.Authserver
{
    internal class Program
    {
        private NetManager _netManager;
        private EventBasedNetListener _netEventListener;
        private static NetPacketProcessor _netProcessor;

        public Program()
        {
            Console.Title = "Authserver";



            Console.WriteLine("Deleting all sessions...");
            using (var ctx = new AuthContext())
                ctx.Accounts.Where(a => a.SessionId != string.Empty)
                    .ExecuteUpdate(setters => setters // todo: see if we can make use of more shorthands like this so we don't need to type out raw SQL code.
                        .SetProperty(p => p.SessionId, default(string)));

            Console.WriteLine("Verifying default account integrity...");
            using (var ctx = new AuthContext())
            {
                if (!ctx.Accounts.Any(a => a.Username.ToUpper().Equals("ADMIN")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "admin".ToUpper(),
                        HashedPassword = "123",
                        SecurityLevel = (int)SecurityLevel.Administrator
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToUpper().Equals("GAMEMASTER")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "gamemaster".ToUpper(),
                        HashedPassword = "123",
                        SecurityLevel = (int)SecurityLevel.Gamemaster
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToUpper().Equals("PLAYER")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "player".ToUpper(),
                        HashedPassword = "123",
                        SecurityLevel = (int)SecurityLevel.Player
                    });
                }

                ctx.SaveChanges();
            }

            // todo: set a configuration setting for using default realms.
            Console.WriteLine("Verifying default realmlist integrity...");
            using (var ctx = new AuthContext())
            {
                if (ctx.Realmlist.Count() == 0)
                {
                    ctx.Add(new Realmserver()
                    {
                        Name = "Test PTR",
                        Hostname = "127.0.0.1",
                        Port = 3733,
                        Flag = (int)RealmFlags.IsPTR | (int)RealmFlags.IsRestricted
                    });
                    ctx.SaveChanges();
                }

                Console.WriteLine($"Registered {ctx.Realmlist.Count()} realm(s).");
            }

            _netProcessor = new NetPacketProcessor();

            _netEventListener = new EventBasedNetListener();
            _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.NetworkReceiveEvent += (peer, reader, delivery) => _netProcessor.ReadAllPackets(reader, peer);

            _netProcessor.SubscribeReusable<RealmAuth_Disconnection, NetPeer>((disconnect, peer) =>
            {
                using (var ctx = new AuthContext())
                {
                    ctx.Accounts.Where(a => a.Id == disconnect.AccountId).ExecuteUpdate(setters => setters.SetProperty(p => p.SessionId, default(string)));
                    Console.WriteLine($"Account ID: {disconnect.AccountId} has disconnected.");
                }
            });

            _netProcessor.SubscribeReusable<RealmAuth_Registrar, NetPeer>((newAuthRegistration, peer) =>
            {
                using (var ctx = new AuthContext())
                {
                    var realms = ctx.Realmlist.ToList();
                    var storedRealm = realms.FirstOrDefault(r => r.StoredEndPoint.Equals(new IPEndPoint(IPAddress.Parse(newAuthRegistration.Ip), newAuthRegistration.Port)));

                    if (storedRealm != null)
                    {
                        Console.WriteLine($"Realmserver ({storedRealm.StoredEndPoint}) has come online.");
                    }
                    else
                    {
                        Console.WriteLine("An unregistered realm is attempting to connect to this authentication server.");
                        peer.Disconnect(); // what happens to the realmserver at this point?
                    }
                }
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
                        Console.WriteLine("Invalid login.");
                    }
                    else
                    {
                        loginCode.Code = LogonCode.AlreadyOnline;
                    }

                    Send(peer, loginCode);

                    if (loginCode.Code == LogonCode.Success)
                    {
                        Send(peer, new AuthClient_Logon() { SessionId = account.SessionId });

                        var realms = new List<RemoteRealmserver>();

                        foreach (var realm in ctx.Realmlist)
                            realms.Add(new RemoteRealmserver(realm.Name, realm.Hostname, realm.Port));
                        SendSerializable(peer, new AuthClient_Realm() { Realmlist = realms });
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
