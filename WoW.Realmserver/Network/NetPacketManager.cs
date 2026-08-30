using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using MySqlX.XDevAPI;
using Nez;
using Nez.BitmapFonts;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Database.Models.Realm.Character;
using WoW.Framework.Logging;
using WoW.Framework.Network.Container;
using WoW.Framework.Shared.Components;
using WoW.Realmserver.Components;
using static WoW.Framework.Network.NetworkManager;
using static WoW.Framework.Utils;

namespace WoW.Realmserver.Network
{
    /// <summary>
    /// Handles reading and writing packet data.
    /// </summary>
    internal class NetPacketManager
    {
        #region Readers

        /// <summary>
        /// Handles clients that are transferring from the authserver after selecting a relam on the realmlist.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="deliveryMethod"></param>
        public static void ReadSessionTransfer(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            string sessionId = reader.GetString();
            peer.Tag = sessionId;

            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.SMSG_REALM_SESSION_TRANSFER_AUTH);
            writer.Put(sessionId);

            Global.Network.SendUnconnected(writer, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8070));
        }
        
        /// <summary>
        /// Handles a confirmation from the authentication server in regard to transfers to the realmserver.
        /// </summary>
        /// <param name="reader"></param>
        public static void ReadSessionTransferConfirmation(NetDataReader reader)
        {
            var isValidTransfer = reader.GetBool();

            if (isValidTransfer)
            {
                int accountId = reader.GetInt();
                string sessionid = reader.GetString();

                var peer = Global.Network.GetPeerWithTag(sessionid);
                int serverId = peer.Id;
                string networkId = Guid.NewGuid().ToString().Replace("-", "");

                Logger.Print($"Account '{accountId}' has entered the realmserver.", LogEntryType.Network);

                SessionComponent newSession = new SessionComponent(accountId, serverId, networkId, sessionid);
                newSession.NetworkState = Components.SessionState.OnCharacterList;
                Entity newPlayerEntity = CoreHeadless.Scene.CreateEntity($"{newSession.AccountId}_{newSession.NetworkId}_{newSession.SessionId})");
                newPlayerEntity.AddComponent(newSession);
                newPlayerEntity.Tag = (int)ActorType.Player;
                peer.Tag = newPlayerEntity;

                NetPacketManager.BuildCharacterList(newSession, peer);
            }
        }

        /// <summary>
        /// Handles a client that is attempting to enter the world with the given character.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="deliveryMethod"></param>
        public static void ReadEnterWorld(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            int characterId = reader.GetInt();
            Entity playerEntity = (Entity)peer.Tag;
            SessionComponent playerSession = playerEntity.GetComponent<SessionComponent>();
            playerSession.SetSelectedCharacter(characterId);
            playerSession.AddComponent(new SpeedComponent(Convert.ToSingle(ConfigurationManager.AppSettings["default_speed"])));

            Logger.Print($"Account '{playerSession.AccountId}' is attempting to use character: ({playerSession.GetSelectedCharacter().Name})", Framework.Utils.LogEntryType.Debug);

            BuildEnterWorld(playerSession, peer);
        }

        /// <summary>
        /// Handles a client updating their position using an axis input.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="deliveryMethod"></param>
        public static void ReadMovementUpdate(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            float xAxis = reader.GetFloat();
            float yAxis = reader.GetFloat();
            long timeTick = reader.GetLong();

            Entity playerEntity = (Entity)peer.Tag;
            SessionComponent playerSession = playerEntity.GetComponent<SessionComponent>();
            playerSession.EnqueuePositionChange(new Vector2(xAxis, yAxis), timeTick);
        }

        public static void ReadChatMessage(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            string clientInput = reader.GetString();

            if (!clientInput.StartsWith("."))
                Logger.Print($"Client is attempting to send chat message: {clientInput}", LogEntryType.Debug);
            else
            {
                clientInput = clientInput.Substring(1, clientInput.Length - 1);
                Logger.Print($"Client is attempting to execute the command string: {clientInput}", LogEntryType.Debug);
            }
        }
        #endregion

        #region Writers

        /// <summary>
        /// Produces the list of characters that belong to a player using their account Id.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="peer"></param>
        public static void BuildCharacterList(SessionComponent newSession, NetPeer peer)
        {
            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.SMSG_REALM_CHARACTER_LIST);

            using (var ctx = new RealmContext())
            {
                List<PlayerCharacter> thisAccountCharacters = ctx.Characters.Where(character => character.AccountId == newSession.AccountId).ToList();
                newSession.Characters = thisAccountCharacters;

                var characterCount = thisAccountCharacters.Count;

                writer.Put(characterCount);

                for (int i = 0; i < characterCount; i++)
                {
                    var thisCharacter = thisAccountCharacters[i];

                    writer.Put(thisCharacter.Name);
                    writer.Put(thisCharacter.Id);
                    writer.Put(thisCharacter.RaceId);
                    writer.Put(thisCharacter.HairId);
                    writer.Put(thisCharacter.MapId);
                    writer.Put(thisCharacter.Direction);
                }
            }
            Global.Network.SendToClient(peer, writer);
        }

        /// <summary>
        /// Confirms the character this player wants to use.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="peer"></param>
        public static void BuildEnterWorld(SessionComponent session, NetPeer peer)
        {
            session.InitializeGameComponents();
            PlayerCharacter character = session.GetSelectedCharacter();

            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.SMSG_REALM_ENTER_WORLD);
            writer.Put(session.GetSelectedCharacter().Id);
            writer.Put(character.XPosition);
            writer.Put(character.YPosition);
            writer.Put(session.NetworkId);
            writer.Put(session.GetComponent<SpeedComponent>().Speed);

            Global.Network.SendToClient(peer, writer);

            BuildCreateAllNetworkedPlayers(peer, session);
            BuildCreateNetworkedPlayer(session); // send the new player to all other players.
            session.NetworkState = Components.SessionState.OnRealm;
        }

        /// <summary>
        /// Builds and sends a new character login to all players.
        /// </summary>
        private static void BuildCreateNetworkedPlayer(SessionComponent newSession)
        {
            Logger.Print($"Sending the new character: '{newSession.GetSelectedCharacter().Name}' to all players.", LogEntryType.Debug);

            NetDataWriter writer = new NetDataWriter(true);
            PlayerCharacter character = newSession.GetSelectedCharacter();

            writer.Put((byte)PacketOpCode.SMSG_REALM_CREATE_ACTOR);
            writer.Put((byte)ActorType.Player);
            writer.Put(newSession.NetworkId);
            writer.Put(character.Name);
            writer.Put(character.HairId);
            writer.Put(character.RaceId);
            writer.Put(character.MapId);
            writer.Put(character.XPosition);
            writer.Put(character.YPosition);
            writer.Put(newSession.GetCharacterSpeed());

            Global.Network.SendToAllExcept(writer, newSession.ServerId);
        }

        /// <summary>
        /// Builds and sends a list of all online characters to the new connection.
        /// </summary>
        /// <param name="newSession"></param>
        private static void BuildCreateAllNetworkedPlayers(NetPeer peer, SessionComponent newSession)
        {
            Logger.Print($"Sending all online characters to: '{newSession.GetSelectedCharacter().Name}'.", LogEntryType.Debug);

            List<SessionComponent> allOtherSessions = 
                CoreHeadless.Scene
                .FindComponentsOfType<SessionComponent>()
                .Where(session => session.ServerId != peer.Id)
                .ToList();

            if (allOtherSessions.Count > 0)
            {
                for (int i = 0; i < allOtherSessions.Count; i++)
                {
                    NetDataWriter writer = new NetDataWriter(true);

                    writer.Put((byte)PacketOpCode.SMSG_REALM_CREATE_ACTOR);
                    var thisOtherSession = allOtherSessions[i];
                    var thisOtherCharacter = thisOtherSession.GetSelectedCharacter();

                    Logger.Print($"Sending {thisOtherCharacter.Name} to {newSession.GetSelectedCharacter().Name}.", LogEntryType.Debug);

                    writer.Put((byte)ActorType.Player);
                    writer.Put(thisOtherSession.NetworkId);
                    writer.Put(thisOtherCharacter.Name);
                    writer.Put(thisOtherCharacter.HairId);
                    writer.Put(thisOtherCharacter.RaceId);
                    writer.Put(thisOtherCharacter.MapId);
                    writer.Put(thisOtherSession.Entity.Position.X);
                    writer.Put(thisOtherSession.Entity.Position.Y);
                    writer.Put(thisOtherSession.GetCharacterSpeed());

                    Global.Network.SendToClient(peer, writer);
                }
            }
        }

        public static void BuildReconciliation(int peerId, Vector2 finalPosition, long timeTick)
        {
            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.SMSG_REALM_MOVE_RECONCILE);
            writer.Put(finalPosition.X);
            writer.Put(finalPosition.Y);
            writer.Put(timeTick);

            Global.Network.SendToClient(peerId, writer); // hack: does movement reconciliation need to be sent as reliable?
        }

        /// <summary>
        /// Updates a session's position change with all players AFTER input has been processed on the server.
        /// </summary>
        /// <param name="thisSession"></param>
        /// <param name="serverProcessedInput"></param>
        public static void BuildPlayerPositionChange(SessionComponent thisSession, Vector2 serverProcessedInput)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)PacketOpCode.SMSG_REALM_MOVE);
            writer.Put(thisSession.NetworkId);
            writer.Put(serverProcessedInput.X);
            writer.Put(serverProcessedInput.Y);

            Global.Network.SendToAllExcept(writer, thisSession.ServerId, DeliveryMethod.Unreliable);
        }

        #endregion
    }
}
