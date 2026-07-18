using LiteNetLib.Utils;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Framework.Logging;
using WoW.Network;
using WoW.Network.Objects;

namespace WoW.Client.Network
{
    /// <summary>
    /// Handles incoming packet data.
    /// </summary>
    public static class NetPacketManager
    {
        /*
         * When writing packet data, the packet's OpCode always comes first to let the peer know what type of data is coming through.
         */

        #region Writers

        /// <summary>
        /// Builds and sends the logon packet to the Authserver.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void BuildLogon(string[] loginData)
        {
            string username = loginData[0];
            string password = WoW.Framework.Utils.ToSha256(loginData[1]);

            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.CMSG_AUTH_LOGON);
            writer.Put(username);
            writer.Put(password);

            // other details?
            /*
             * ex: game version, OS, etc?
             */ 

            Global.Network.SendToServer(writer);
        }

        #endregion

        #region Readers

        /// <summary>
        /// Handles the response received from <see cref="BuildLogon(string[])"/>
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadLogonResponse(NetDataReader reader)
        {
            PacketOpCode logonCode = (PacketOpCode)reader.GetByte();
            NetFootprintComponent myFootprint = Global._Player.GetComponent<NetFootprintComponent>();

            switch (logonCode)
            {
                case PacketOpCode.SMSG_AUTH_LOGON_SUCCESS:
                    string sessionId = reader.GetString();
                    string accountName = reader.GetString();

                    myFootprint.Create(accountName, sessionId);

                    Logger.Print($"Logged in successfully.", Framework.Utils.LogEntryType.Debug);
                    break;
            }
        }

        /// <summary>
        /// Handles the realmlist packet.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadRealmlist(NetDataReader reader)
        {
            var realmCount = reader.GetInt();

            for (int i = 0; i < realmCount; i++)
            {
                string name = reader.GetString();
                string hostname = reader.GetString();
                int port = reader.GetInt();

                RealmserverMetadataObject newRealm = new RealmserverMetadataObject(name, hostname, port);
                Global.Realmlist.Add(newRealm);
            }

            Logger.Print($"Processed {realmCount} realm(s).", Framework.Utils.LogEntryType.Debug);

            // display the realmlist.
            Global.PeerState = Global.GameNetworkState.Auth_Realmlist;
        }

        /// <summary>
        /// Handles the character list data from the server. This function also allows for rendering of the character list.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadCharacterList(NetDataReader reader)
        {
            int count = reader.GetInt();

            Logger.Print($"Receiving data for {count} character(s).", Framework.Utils.LogEntryType.Debug);

            List<CharacterMetadataObject> characters = new List<CharacterMetadataObject>();

            for (int i = 0; i < count; i++)
            {
                string name = reader.GetString();
                int id = reader.GetInt();
                int raceId = reader.GetInt();
                int hairId = reader.GetInt();
                string mapId = reader.GetString();
                float xPos = reader.GetFloat();
                float yPos = reader.GetFloat();
                int direction = reader.GetInt();

                characters.Add(
                    new CharacterMetadataObject(id, name, (Framework.Utils.ActorRaceType)raceId, hairId, mapId: mapId));

                Global.Characters = characters;
                Global.PeerState = Global.GameNetworkState.Realm_Characters;
            }
        }

        #endregion
    }
}
