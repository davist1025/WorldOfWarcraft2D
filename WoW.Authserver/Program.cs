using Isopoh.Cryptography.Argon2;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
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

            using (var ctx = new AuthContext())
            {
                Console.WriteLine("Deleting all sessions...");
                ctx.Accounts.Where(a => a.SessionId != string.Empty)
                    .ExecuteUpdate(setters => setters
                        .SetProperty(p => p.SessionId, default(string)));

                Console.WriteLine("Verifying default account integrity...");
                if (!ctx.Accounts.Any(a => a.Username.ToLower().Equals("admin")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "admin".ToUpper(),
                        HashedPassword = Argon2.Hash(Utils.ToSHA256("123")),
                        SecurityLevel = (int)SecurityLevel.Administrator
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToLower().Equals("gamemaster")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "gamemaster".ToUpper(),
                        HashedPassword = Argon2.Hash(Utils.ToSHA256("456")),
                        SecurityLevel = (int)SecurityLevel.Gamemaster
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToLower().Equals("player")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "player".ToUpper(),
                        HashedPassword = Argon2.Hash(Utils.ToSHA256("789")),
                        SecurityLevel = (int)SecurityLevel.Player
                    });
                }

                ctx.SaveChanges();

                // todo: set a configuration setting for using default realms.
                Console.WriteLine("Verifying default realmlist integrity...");
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

            _netProcessor.SubscribeReusable<RealmAuth_Disconnection, NetPeer>((disconnect, peer) => PacketManager.OnPlayerDisconnect(disconnect));

            _netProcessor.SubscribeReusable<RealmAuth_Registrar, NetPeer>((newAuthRegistration, peer) => PacketManager.OnRealmRegister(newAuthRegistration, peer));

            _netProcessor.SubscribeReusable<ClientAuth_Logon, NetPeer>((newAuth, peer) => PacketManager.OnUserLogin(newAuth, peer));

            _netProcessor.SubscribeReusable<RealmAuth_SessionVerification, NetPeer>((request, peer) => PacketManager.OnSessionVerification(request, peer));

            _netManager = new NetManager(_netEventListener);
            _netManager.Start(8070);

            while (true)
            {
                _netManager.PollEvents();
            }
        }

        public static void Send<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(peer, packet, delivery);

        public static void SendSerializable<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => _netProcessor.SendNetSerializable(peer, packet, delivery);

        static void Main(string[] args)
            => new Program();
    }
}
