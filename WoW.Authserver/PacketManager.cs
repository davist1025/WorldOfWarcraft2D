using LiteNetLib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WoW.Authserver.DB;
using WoW.Client.Shared.Auth;
using WoW.Client.Shared;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Data;
using WoW.Server.Shared;
using Org.BouncyCastle.Asn1.Ocsp;
using WoW.Authserver.DB.Model;
using WoW.Server.Shared.Serializable;
using Isopoh.Cryptography.Argon2;

namespace WoW.Authserver
{
    public static class PacketManager
    {
        public static void OnUserLogin(ClientAuth_Logon logon, NetPeer peer) 
        {
            Console.WriteLine($"{logon.AccountName} is trying to log in...");

            using (var ctx = new AuthContext())
            {
                AuthClient_LogonCode loginCode = new AuthClient_LogonCode();
                string accountSessionId = "";
                var account = ctx.Accounts.FirstOrDefault(a => a.Username.ToLower().Equals(logon.AccountName));

                if (account != null && account.SessionId == default(string))
                {
                    Console.WriteLine($"Verifying password of {logon.AccountName}...");

                    if (Argon2.Verify(account.HashedPassword, logon.Password))
                    {
                        Console.WriteLine($"Generating session for {logon.AccountName}...");
                        accountSessionId = Guid.NewGuid().ToString().Replace("-", "");
                        account.SessionId = accountSessionId;
                        loginCode.Code = LogonCode.Success;
                    }
                    else
                        loginCode.Code = LogonCode.InvalidPassword;
                }
                else if (account == null)
                {
                    loginCode.Code = LogonCode.NoRecord;
                }
                else
                {
                    loginCode.Code = LogonCode.AlreadyOnline;
                }

                Program.Send(peer, loginCode);

                if (loginCode.Code == LogonCode.Success)
                {
                    Program.Send(peer, new AuthClient_Logon() { SessionId = account.SessionId });

                    var realms = new List<RemoteRealmserver>();

                    foreach (var realm in ctx.Realmlist)
                        realms.Add(new RemoteRealmserver(realm.Name, realm.Hostname, realm.Port));
                    Program.SendSerializable(peer, new AuthClient_Realm() { Realmlist = realms });
                }

                ctx.SaveChanges();
            }
        }

        public static void OnSessionVerification(RealmAuth_SessionVerification verification, NetPeer peer)
        {
            using (var ctx = new AuthContext())
            {
                Account account = ctx.Accounts.FirstOrDefault(a => a.SessionId.ToLower().Equals(verification.SessionId));
                if (account != null)
                {
                    Console.WriteLine("Sending user verification to realm...");
                    Program.SendSerializable(peer, new AuthRealm_SessionVerification()
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
        }

        public static void OnPlayerDisconnect(RealmAuth_Disconnection disconnection)
        {
            using (var ctx = new AuthContext())
            {
                ctx.Accounts.Where(a => a.Id == disconnection.AccountId).ExecuteUpdate(setters => setters.SetProperty(p => p.SessionId, default(string)));
                Console.WriteLine($"Account ID: {disconnection.AccountId} has disconnected.");
            }
        }

        public static void OnRealmRegister(RealmAuth_Registrar realmData, NetPeer peer)
        {
            using (var ctx = new AuthContext())
            {
                var realms = ctx.Realmlist.ToList();
                var storedRealm = realms.FirstOrDefault(r => r.StoredEndPoint.Equals(new IPEndPoint(IPAddress.Parse(realmData.Ip), realmData.Port)));

                if (storedRealm != null)
                {
                    Console.WriteLine($"Realmserver ({storedRealm.StoredEndPoint}) has come online.");
                    // does this need additional security?
                }
                else
                {
                    Console.WriteLine("An unregistered realm is attempting to connect to this authentication server.");
                    peer.Disconnect(); // what happens to the realmserver at this point?
                }
            }
        }
    }
}
