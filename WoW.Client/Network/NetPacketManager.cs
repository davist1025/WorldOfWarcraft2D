using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Client.Scenes;
using WoW.Framework.Logging;
using WoW.Framework.Network.Container;
using WoW.Framework.Shared;
using WoW.Framework.Shared.Components;
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
        public static void BuildEnterWorld(CharacterContainer character)
        {
            Logger.Print($"Attempting to play on character: ({character.Name})", LogEntryType.Debug);

            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.CMSG_REALM_ENTER_WORLD);
            writer.Put(character.Id);

            Global.Network.SendToServer(writer);
        }

        /// <summary>
        /// Builds and sends a packet for local player movement updates (you!)
        /// </summary>
        /// <param name="input"></param>
        public static void BuildMovementUpdate(Vector2 input)
        {
            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.CMSG_REALM_MOVE);
            writer.Put(input.X);
            writer.Put(input.Y);

            Global.Network.SendToServer(writer, DeliveryMethod.Unreliable);
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
                    string sessionId = reader.GetString();
                    string accountName = reader.GetString();
                    NetFootprintComponent myNetFootprint = new NetFootprintComponent(accountName, sessionId);
                    Global._Player.AddComponent(myNetFootprint);

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
            NetFootprintComponent footprint = Global._Player.GetComponent<NetFootprintComponent>();

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
            }

            footprint.SetCharacterList(characters);
            footprint.SetSelectedCharacter(0);

            Global.PeerState = Global.GameNetworkState.Realm_Characters;
        }

        /// <summary>
        /// Handles the server's "ok" to enter the game world.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadEnterWorld(NetDataReader reader)
        {
            int index = reader.GetInt();
            float x = reader.GetFloat();
            float y = reader.GetFloat();
            string myNetworkId = reader.GetString();
            float defaultSpeed = reader.GetFloat();

            NetFootprintComponent footprint = Global._Player.GetComponent<NetFootprintComponent>();
            footprint.SetSelectedCharacter(index);
            footprint.Entity.AddComponent(new SpeedComponent(defaultSpeed));

            Global._Player.SetPosition(new Vector2(x, y));
            Global.PeerState = Global.GameNetworkState.World;
            Global.NetworkId = myNetworkId;

            Core.StartSceneTransition(new FadeTransition(() => new WorldScene()));
        }

        /// <summary>
        /// Handles a new actor being created on the server.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadNewActor(NetDataReader reader)
        {
            ActorType actorType = (ActorType)reader.GetByte();
            Entity newNetworkedActor = null;

            Logger.Print("New actor!!", LogEntryType.Debug);

            switch (actorType)
            {
                case ActorType.Player:
                    string networkId = reader.GetString();

                    // character info.
                    string name = reader.GetString(); // name
                    int hairId = reader.GetInt(); // hair id
                    int raceId =  reader.GetInt(); // race id
                    string mapId = reader.GetString(); // map id
                    float x = reader.GetFloat(); // x
                    float y = reader.GetFloat(); // y

                    newNetworkedActor = ClientCore.Scene.CreateEntity(networkId);
                    OnlinePlayerData playerData = new OnlinePlayerData(networkId, name, hairId, raceId, mapId, x, y);
                    newNetworkedActor.AddComponent(new OnlinePlayerControllerComponent(playerData));
                    break;
            }
        }

        public static void ReadMovementUpdate(NetDataReader reader)
        {
            string networkId = reader.GetString();
            float x = reader.GetFloat();
            float y = reader.GetFloat();

            var onlinePlayer = ClientCore.Scene
                .FindComponentsOfType<OnlinePlayerControllerComponent>()
                .Where(player => string.Equals(player.Data.NetworkId, networkId, StringComparison.OrdinalIgnoreCase))
                .Single();
            onlinePlayer.EnqueuePositionChange(new Vector2(x, y));
        }
        #endregion
    }
}
