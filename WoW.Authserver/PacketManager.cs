using Isopoh.Cryptography.Argon2;
using LiteNetLib;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Framework.Logging;
using WoW.Network.Objects;
using WoW.Network.Packets.Authenticcation;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using static WoW.Framework.Utils;

namespace WoW.Authserver
{
    public static class PacketManager
    {
        public static void OnUserLogin(ClientAuth_Logon logon, NetPeer peer) 
        {
            Logger.Print($"{logon.AccountName} is attempting to log-in...", LogEntryType.Network);

            using (var ctx = new AuthContext())
            {
                AuthClient_LogonCode loginCode = new AuthClient_LogonCode();
                string accountSessionId = "";
                var account = ctx.Accounts.FirstOrDefault(a => a.Username.ToLower().Equals(logon.AccountName));

                if (account != null && account.SessionId == "-")
                {
                    if (Argon2.Verify(account.HashedPassword, logon.Password))
                    {
                        Logger.Print($"{logon.AccountName} logged in successfully; generating a new session key...", Framework.Utils.LogEntryType.Network);
                        accountSessionId = Guid.NewGuid().ToString().Replace("-", "");
                        account.SessionId = accountSessionId;
                        loginCode.Code = Framework.Utils.AuthCodeType.Success;
                    }
                    else
                        loginCode.Code = Framework.Utils.AuthCodeType.InvalidPassword;
                }
                else if (account == null)
                {
                    loginCode.Code = Framework.Utils.AuthCodeType.NoRecord;
                }
                else
                {
                    loginCode.Code = Framework.Utils.AuthCodeType.AlreadyOnline;
                }

                Program.Send(peer, loginCode);

                if (loginCode.Code == Framework.Utils.AuthCodeType.Success)
                {
                    Program.Send(peer, new AuthClient_Logon() { SessionId = account.SessionId });

                    var realms = new List<RealmserverMetadataObject>();

                    foreach (var realm in ctx.Realmlist)
                        realms.Add(new RealmserverMetadataObject(realm.Name, realm.Hostname, realm.Port));
                    Program.SendSerializable(peer, new AuthClient_Realm() { Realmlist = realms });
                }

                ctx.SaveChanges();
            }
        }

        [Obsolete("Unused 1/18/26.")]
        public static void OnRealmRegister(RealmAuth_Registrar realmData, NetPeer peer)
        {
            using (var ctx = new AuthContext())
            {
                var realms = ctx.Realmlist.ToList();
                var storedRealm = realms.FirstOrDefault(r => r.StoredEndPoint.Equals(new IPEndPoint(IPAddress.Parse(realmData.Ip), realmData.Port)));

                if (storedRealm != null)
                {
                    Logger.Print($"{storedRealm.Name} has come online.", LogEntryType.Network);
                    // does this need additional security?
                }
                else
                    peer.Disconnect();
            }
        }
    }
}
