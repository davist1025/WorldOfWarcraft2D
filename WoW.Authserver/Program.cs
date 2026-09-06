using Isopoh.Cryptography.Argon2;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using WoW.Database.Models;
using WoW.Database.Models.Auth;
using WoW.Framework;
using WoW.Framework.Logging;
using WoW.Framework.Network;
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
        public Program()
        {
            Console.Title = "Authserver";

            using (var ctx = new AuthContext())
            {
                Logger.Print("Resetting session keys...", LogEntryType.Process);
                // hack: probably not a proper way of resetting the session.

                ctx.Accounts
                    .Where(user => user.SessionId != "-")
                    .ExecuteUpdate(userProp => userProp
                        .SetProperty(property => property.SessionId, "-"));

                // todo: add flag in config for debug account usage.
                Logger.Print("Verifying debug account integrity...", LogEntryType.Process);
                if (!ctx.Accounts.Any(a => a.Username.ToUpper().Equals("ADMIN")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "ADMIN",
                        HashedPassword = Argon2.Hash(Utils.ToSha256("123")),
                        SecurityLevel = (int)AccountSecurityType.Administrator
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToUpper().Equals("GAMEMASTER")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "GAMEMASTER",
                        HashedPassword = Argon2.Hash(Utils.ToSha256("456")),
                        SecurityLevel = (int)AccountSecurityType.Gamemaster
                    });
                }

                if (!ctx.Accounts.Any(a => a.Username.ToLower().Equals("PLAYER")))
                {
                    ctx.Accounts.Add(new Account()
                    {
                        Username = "PLAYER",
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
                        Port = 3733
                    });
                    ctx.SaveChanges();
                }

                Logger.Print($"Registered {ctx.Realmlist.Count()} realm(s).", LogEntryType.Process);
            }

            Global.Network = new NetworkManager(new NetworkEventListener());
            Global.Network.StartServer(ConfigurationManager.AppSettings["hostname"].Split(":"));

            while (true)
                Global.Network.Update();
        }

        static void Main(string[] args)
            => new Program();
    }
}
