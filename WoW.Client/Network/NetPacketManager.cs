using LiteNetLib.Utils;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Client.Scenes;
using WoW.Framework.Logging;
using WoW.Framework.Network.Container;
using static WoW.Framework.Network.NetworkManager;
using static WoW.Framework.Utils;

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

        /// <summary>
        /// Builds and sends a packet dictating which character we would like to play on.
        /// </summary>
        public static void BuildEnterWorld(CharacterContainer selectedCharacter)
        {
            Logger.Print($"Attempting to play on character: ({selectedCharacter.Name})", LogEntryType.Debug);

            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.CMSG_REALM_ENTER_WORLD);
            writer.Put(selectedCharacter.Id);

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

            switch (logonCode)
            {
                case PacketOpCode.SMSG_AUTH_LOGON_SUCCESS:
                    NetFootprintComponent myFootprint = Global._Player.AddComponent<NetFootprintComponent>();
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

                RealmserverContainer newRealm = new RealmserverContainer(name, hostname, port);
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

            List<CharacterContainer> characters = new List<CharacterContainer>();

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
                    new CharacterContainer(name, id, (ActorRaceType)raceId, hairId, mapId: mapId, xPos, yPos, (ActorAnimationDirection)direction));

                Global.Characters = characters;
                Global.PeerState = Global.GameNetworkState.Realm_Characters;
            }
        }

        public static void ReadEnterWorld(NetDataReader reader)
        {
            int index = reader.GetInt();
            Global.SetSelectedCharacter(index);

            Core.StartSceneTransition(new FadeTransition(() => new WorldScene()));
        }
        #endregion
    }
}
