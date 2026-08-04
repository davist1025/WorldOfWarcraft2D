using Isopoh.Cryptography.Argon2;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Framework;
using WoW.Framework.Logging;
using static WoW.Framework.Network.NetworkManager;

namespace WoW.Authserver
{
    /// <summary>
    /// Handles reading and writing packet data.
    /// </summary>
    internal class NetPacketManager
    {
        #region Readers

        /// <summary>
        /// Handles a new client logon.
        /// 
        /// Logons will get declined for invalid passwords, active sessions, bans, etc.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="deliveryMethod"></param>
        public static void ReadLogin(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            Logger.Print($"Handling logon packet from: {peer.EndPoint}.", Utils.LogEntryType.Debug);

            string username = reader.GetString();
            string sha256Password = reader.GetString();

            // init the response packet.
            NetDataWriter response = new NetDataWriter(true);
            response.Put((byte)PacketOpCode.SMSG_AUTH_LOGON);

            // run the database check on this user.
            using (var ctx = new AuthContext())
            {
                var account = ctx.Accounts.FirstOrDefault(account => account.Username.ToUpper().Equals(username));

                if (account != null)
                {
                    if (!account.SessionId.Equals("-"))
                    {
                        // todo: [logon handler] account already in use.
                        return;
                    }

                    // account exists and the password matches.
                    if (Argon2.Verify(account.HashedPassword, sha256Password))
                    {
                        Logger.Print($"{account.Username} is logging in.", Utils.LogEntryType.Network);

                        account.SessionId = Guid.NewGuid().ToString().Replace("-", "");

                        response.Put((byte)PacketOpCode.SMSG_AUTH_LOGON_SUCCESS);
                        response.Put(account.SessionId);
                        response.Put(account.Username.ToLower());

                        // update the session id and commit to the db.
                        ctx.SaveChanges();
                    }

                    // send the logon packet.
                    Global.Network.SendToClient(peer, response);
                }

                NetDataWriter realmlistWriter = new NetDataWriter(true);
                realmlistWriter.Put((byte)PacketOpCode.SMSG_AUTH_REALMLIST);

                realmlistWriter.Put(ctx.Realmlist.Count());
                foreach (var realm in ctx.Realmlist)
                {
                    realmlistWriter.Put(realm.Name);
                    realmlistWriter.Put(realm.Hostname);
                    realmlistWriter.Put(realm.Port);
                }

                // send the realmlist packet.
                Global.Network.SendToClient(peer, realmlistWriter);
            }
        }

        public static void ReadClientDisconnection(NetDataReader reader)
        {
            string clientSessionId = reader.GetString();

            using (var authCtx = new AuthContext())
            {
                var thisAccount = authCtx.Accounts.Single(account => string.Equals(account.SessionId.ToLower(), clientSessionId.ToLower()));
                thisAccount.SessionId = "-";
                authCtx.SaveChanges();
            }

            Logger.Print($"Client '{clientSessionId}' has disconnected.", Utils.LogEntryType.Network);
        }

        #endregion
    }
}
