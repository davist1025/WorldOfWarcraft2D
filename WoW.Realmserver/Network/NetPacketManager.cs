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

            using (var ctx = new AuthContext())
            {
                if (ctx.Accounts.Any(account => account.SessionId.Equals(sessionId)))
                {
                    var accountData = ctx.Accounts.Where(account => account.SessionId.Equals(sessionId)).Single();

                    SessionComponent newSession = new SessionComponent(accountData);
                    newSession.NetworkId = peer.Id;
                    newSession.NetworkState = Components.SessionState.OnCharacterList;
                    Entity newPlayerEntity = CoreHeadless.Scene.CreateEntity($"{accountData.Username}({accountData.SessionId})");
                    newPlayerEntity.AddComponent(newSession);
                    newPlayerEntity.Tag = (int)ActorType.Player;
                    peer.Tag = newPlayerEntity;

                    Logger.Print($"Account '{accountData.Username}' with session id ({accountData.SessionId}) has successfully entered the realm.", LogEntryType.Debug);

                    NetPacketManager.BuildCharacterList(newSession, peer);
                }
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

            Logger.Print($"Account ({playerSession.Account.Username}) is attempting to use character: ({playerSession.GetSelectedCharacter().Name})", Framework.Utils.LogEntryType.Debug);

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

            Entity playerEntity = (Entity)peer.Tag;
            SessionComponent playerSession = playerEntity.GetComponent<SessionComponent>();
            playerSession.EnqueuePositionChange(new Vector2(xAxis, yAxis));

            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)PacketOpCode.SMSG_REALM_MOVE);

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
                List<PlayerCharacter> thisAccountCharacters = ctx.Characters.Where(character => character.AccountId == newSession.Account.Id).ToList();
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
                    writer.Put(thisCharacter.XPosition);
                    writer.Put(thisCharacter.YPosition);
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
            writer.Put(session.GetComponent<SpeedComponent>().Speed);

            Logger.Print($"Confirmed the player's choice of character: ({session.GetSelectedCharacter().Name})", Framework.Utils.LogEntryType.Debug);

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
            writer.Put(character.Name);
            writer.Put(character.HairId);
            writer.Put(character.RaceId);
            writer.Put(character.MapId);
            writer.Put(character.XPosition);
            writer.Put(character.YPosition);

            Global.Network.SendToAllExcept(writer, newSession.NetworkId);
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
                .Where(session => session.NetworkId != newSession.NetworkId)
                .ToList();

            if (allOtherSessions.Count > 0)
            {
                NetDataWriter writer = new NetDataWriter(true);

                writer.Put((byte)PacketOpCode.SMSG_REALM_CREATE_ACTOR);
                writer.Put(allOtherSessions.Count);

                for (int i = 0; i < allOtherSessions.Count; i++)
                {
                    var thisOtherSession = allOtherSessions[i];
                    var character = thisOtherSession.GetSelectedCharacter();

                    writer.Put((byte)ActorType.Player);
                    writer.Put(newSession.NetworkId);
                    writer.Put(character.Name);
                    writer.Put(character.HairId);
                    writer.Put(character.RaceId);
                    writer.Put(character.MapId);
                    writer.Put(character.XPosition);
                    writer.Put(character.YPosition);
                }

                Global.Network.SendToClient(peer, writer);
            }
        }

        #endregion
    }
}
