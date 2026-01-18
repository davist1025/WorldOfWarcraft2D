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
using WoW.Database.Models;
using WoW.Database.Models.Auth;
using WoW.Framework;
using WoW.Framework.Logging;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using static WoW.Framework.Utils;

namespace WoW.Authserver
{
    /** Password hashing **/
    /* 10-13-25
     *  
     * (Client) - Hash a plaintext password w/ Sha-256.
     * (Server) - Hash a sha-256 hashed password with Argon2. This (encoded) value gets written to the database, and verified using Argon2.Verify(x,y).
     * 
     */ 

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
                Logger.Print("Resetting session keys...", LogEntryType.Process);
                // hack: probably not a proper way of resetting the session.
                RelationalQueryableExtensions
                    .ExecuteUpdate(ctx.Accounts.Where(account => account.SessionId != string.Empty), setters => setters.SetProperty(acc => acc.SessionId, "-"));

                // todo: add flag in config for debug account usage.
                Logger.Print("Verifying debug account integrity...", LogEntryType.Process);
                if (!ctx.Accounts.Any(a => a.Username.ToLower().Equals("admin")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "admin".ToUpper(),
                        HashedPassword = Argon2.Hash(Utils.ToSha256("123")),
                        SecurityLevel = (int)AccountSecurityType.Administrator
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToLower().Equals("gamemaster")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "gamemaster".ToUpper(),
                        HashedPassword = Argon2.Hash(Utils.ToSha256("456")),
                        SecurityLevel = (int)AccountSecurityType.Gamemaster
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToLower().Equals("player")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "player".ToUpper(),
                        HashedPassword = Argon2.Hash(Utils.ToSha256("789")),
                        SecurityLevel = (int)AccountSecurityType.Player
                    });
                }

                ctx.SaveChanges();

                // todo: add flag in config for debug realmlist usage.
                Logger.Print("Verifying debug realmist integrity...", LogEntryType.Process);
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

                Logger.Print($"Registered {ctx.Realmlist.Count()} realm(s).", LogEntryType.Process);
            }
            _netProcessor = new NetPacketProcessor();

            _netEventListener = new EventBasedNetListener();
            _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.NetworkReceiveEvent += (peer, reader, delivery) => _netProcessor.ReadAllPackets(reader, peer);

            _netProcessor.SubscribeReusable<RealmAuth_Registrar, NetPeer>((newAuthRegistration, peer) => PacketManager.OnRealmRegister(newAuthRegistration, peer));

            _netProcessor.SubscribeReusable<ClientAuth_Logon, NetPeer>((newAuth, peer) => PacketManager.OnUserLogin(newAuth, peer));

            _netManager = new NetManager(_netEventListener);
            _netManager.Start("127.0.0.1", "", 8070);

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
