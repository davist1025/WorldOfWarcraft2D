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
using WoW.Client.Components.Player;
using WoW.Client.Scenes;
using WoW.Client.Utils;
using WoW.Framework.Logging;
using WoW.Framework.Network;
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
    public class NetPacketManager
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

            QueueablePacket writer = new QueueablePacket(true);
            writer.Put((byte)PacketOpCode.CMSG_AUTH_LOGON);
            writer.Put(username);
            writer.Put(password);

            // other details?
            /*
             * ex: game version, OS, etc?
             */

            Core.GetGlobalManager<GameManager>().QueuePacket(writer);
            //_Core.GetGlobalManager<GameManager>().Network.SendToServer(writer);
        }

        /// <summary>
        /// Builds and sends a packet dictating which character we would like to play on.
        /// </summary>
        public static void BuildEnterWorld(CharacterContainer character)
        {
            Logger.Print($"Attempting to play on character: ({character.Name})", LogEntryType.Debug);

            QueueablePacket writer = new QueueablePacket(true);
            writer.Put((byte)PacketOpCode.CMSG_REALM_ENTER_WORLD);
            writer.Put(character.Id);

            Core.GetGlobalManager<GameManager>().QueuePacket(writer);
            //_Core.GetGlobalManager<GameManager>().Network.SendToServer(writer);
        }

        /// <summary>
        /// Builds and sends a packet for local player movement updates (you!)
        /// </summary>
        /// <param name="input"></param>
        public static void BuildMovementUpdate(Vector2 input, long timeTick)
        {
            QueueablePacket writer = new QueueablePacket(true)
            {
                DeliveryMethod = DeliveryMethod.Unreliable
            };

            writer.Put((byte)PacketOpCode.CMSG_REALM_MOVE);
            writer.Put(input.X);
            writer.Put(input.Y);
            writer.Put(timeTick);

            Core.GetGlobalManager<GameManager>().QueuePacket(writer);
            //_Core.GetGlobalManager<GameManager>().Network.SendToServer(writer, DeliveryMethod.Unreliable);
        }

        public static void BuildChatMessage(string input)
        {
            QueueablePacket writer = new QueueablePacket(true);

            writer.Put((byte)PacketOpCode.CMSG_REALM_CHAT);
            writer.Put(input);

            Core.GetGlobalManager<GameManager>().QueuePacket(writer);
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
                    MyOnlineComponent myNetFootprint = new MyOnlineComponent(accountName, sessionId);
                    Core.GetGlobalManager<GameManager>().Player.AddComponent(myNetFootprint);

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
                Core.GetGlobalManager<GameManager>().Realmlist.Add(newRealm);
            }

            Logger.Print($"Processed {realmCount} realm(s).", Framework.Utils.LogEntryType.Debug);

            // display the realmlist.
            Core.GetGlobalManager<GameManager>().PeerState = GameNetworkState.Auth_Realmlist;
        }

        /// <summary>
        /// Handles the character list data from the server. This function also allows for rendering of the character list.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadCharacterList(NetDataReader reader)
        {
            int count = reader.GetInt();
            MyOnlineComponent footprint = Core.GetGlobalManager<GameManager>().Player.GetComponent<MyOnlineComponent>();

            Logger.Print($"Receiving data for {count} character(s).", Framework.Utils.LogEntryType.Debug);

            List<CharacterContainer> characters = new List<CharacterContainer>();

            for (int i = 0; i < count; i++)
            {
                string name = reader.GetString();
                int id = reader.GetInt();
                int raceId = reader.GetInt();
                int hairId = reader.GetInt();
                string mapId = reader.GetString();
                int direction = reader.GetInt();

                characters.Add(
                    new CharacterContainer(name, id, (ActorRaceType)raceId, hairId, mapId: mapId, (ActorAnimationDirection)direction));
            }

            footprint.SetCharacterList(characters);

            Core.GetGlobalManager<GameManager>().PeerState = GameNetworkState.Realm_Characters;
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

            MyOnlineComponent footprint = Core.GetGlobalManager<GameManager>().Player.GetComponent<MyOnlineComponent>();
            footprint.Entity.AddComponent(new SpeedComponent(defaultSpeed));

            Core.GetGlobalManager<GameManager>().Player.SetPosition(new Vector2(x, y));
            Core.GetGlobalManager<GameManager>().PeerState = GameNetworkState.World;
            Core.GetGlobalManager<GameManager>().NetworkId = myNetworkId;

            Core.StartSceneTransition(new FadeTransition(() => new WorldScene()));
        }

        /// <summary>
        /// Handles a new actor being created on the server.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadNewActor(NetDataReader reader)
        {
            Entity newNetworkedActor = null;
            var gameManager = Core.GetGlobalManager<GameManager>();

            ActorType actorType = (ActorType)reader.GetByte();

            switch (actorType)
            {
                case ActorType.Player:
                    string networkId = reader.GetString();

                    // character info.
                    string name = reader.GetString(); // name
                    int hairId = reader.GetInt(); // hair id
                    int raceId = reader.GetInt(); // race id
                    string mapId = reader.GetString(); // map id
                    float x = reader.GetFloat(); // x
                    float y = reader.GetFloat(); // y
                    float movementSpeed = reader.GetFloat();

                    newNetworkedActor = new Entity(networkId);
                    OnlinePlayerData playerData = new OnlinePlayerData(networkId, name, hairId, raceId, mapId, x, y, movementSpeed);
                    newNetworkedActor.AddComponent(new OnlinePlayerControllerComponent(playerData));
                    break;
            }

            Core.GetGlobalManager<GameManager>().NewActor?.Invoke(null, newNetworkedActor);
        }

        /// <summary>
        /// Handles an entity's movement update.
        /// 
        /// We only process input.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadMovementUpdate(NetDataReader reader)
        {
            string networkId = reader.GetString();
            float x = reader.GetFloat();
            float y = reader.GetFloat();

            var onlinePlayer = ClientCore.Scene
                .FindComponentsOfType<OnlinePlayerControllerComponent>()
                .Single(player => string.Equals(player.Data.NetworkId, networkId, StringComparison.OrdinalIgnoreCase));
            onlinePlayer.EnqueuePositionChange(new Vector2(x, y));
        }

        /// <summary>
        /// Received when the server detects a disconnection.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadDisconnection(NetDataReader reader)
        {
            string sessionId = reader.GetString();
            // todo: (disconnect called on our player) placeholder code.
            //NetFootprintComponent myFootprint = 
            //    (Global.PeerState == Global.GameNetworkState.World) ? Core.Scene.FindComponentOfType<NetFootprintComponent>() : null;

            //// if we were in-game, we need to clear out our character list and other network variables.
            //if (Global.PeerState == Global.GameNetworkState.World 
            //    && myFootprint != null
            //    && string.Equals(sessionId, myFootprint.SessionId, StringComparison.OrdinalIgnoreCase))
            //    Core.GetGlobalManager<GameManager>()>().Disconnected?.Invoke(null, null);

            var onlinePlayer = Core.Scene.Entities.FindEntity(sessionId);

            if (onlinePlayer != null)
            {
                Logger.Print($"'{onlinePlayer.GetComponent<OnlinePlayerControllerComponent>().Data.CharacterName}' has left the world!", LogEntryType.Network);
                onlinePlayer.Destroy();
            }
        }

        /// <summary>
        /// Handles the server's calculation of our position based on input sent previously.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadReconciliation(NetDataReader reader)
        {
            float realX = reader.GetFloat();
            float realY = reader.GetFloat();
            long originalTimeTick = reader.GetLong();

            var gameManager = Core.GetGlobalManager<GameManager>();
            var tuple = gameManager.FindMovementChangeByTick(originalTimeTick);

            //var serverPos = new Vector2(realX, realY);
            //float syncDifference = Vector2.Distance(serverPos, tuple.Item3);

            //if (syncDifference > 1.5f)
            //{
            //    Logger.Print($"We are desynchronized from the server!", LogEntryType.Fatal);

            //    /*
            //     * todo: movement reconciliation [client].
            //     * thinking we need to set an interal position that isn't rendered to 'serverPos',
            //     * grab all client-side movement changes up to now,
            //     * replay all changes from the server's position to now,
            //     * lerp our rendered position to the reconciled position.
            //     */ 
            //}
        }

        public static void ReadChatMessage(NetDataReader reader)
        {
            string clietNetworkId = reader.GetString();
            string input = reader.GetString();
            GameManager gameManager = Core.GetGlobalManager<GameManager>();

            if (string.Equals(gameManager.NetworkId, clietNetworkId, StringComparison.OrdinalIgnoreCase))
            {
                input = $"[{gameManager.Player.GetComponent<MyOnlineComponent>().GetSelectedCharacter().Name}] says: {input}";
                gameManager.ChatHistory.Add(new ChatMessageContainer() { Input = input });
                //Debug.Log($"{gameManager.Player.GetComponent<MyOnlineComponent>().GetSelectedCharacter().Name} says: {input}");
            }
            else
            {
                Entity onlinePlayer = Core.Scene.FindEntity(clietNetworkId);
                input = $"[{onlinePlayer.GetComponent<OnlinePlayerControllerComponent>().Data.CharacterName}] says: {input}";
                gameManager.ChatHistory.Add(new ChatMessageContainer() { Input = input });
            }
        }
        #endregion
    }
}
