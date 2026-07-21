using LiteNetLib;
using LiteNetLib.Utils;
using MySqlX.XDevAPI;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Database.Models.Realm.Character;
using WoW.Framework.Logging;
using WoW.Realmserver.Components;
using static WoW.Framework.Network.NetworkManager;

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

            Global.Transfers.Enqueue(new Tuple<string, NetPeer>(sessionId, peer));

            Logger.Print($"Enqueued ({sessionId}) for transfer processing.", Framework.Utils.LogEntryType.Debug);
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

            Logger.Print($"Account ({playerSession.Account.Username}) is attempting to use character: ({playerSession.GetSelectedCharacter().Name})", Framework.Utils.LogEntryType.Debug);

            BuildEnterWorld(playerSession, peer);
        }

        public static void ReadMovementUpdate(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            float xAxis = reader.GetFloat();
            float yAxis = reader.GetFloat();

            Entity playerEntity = (Entity)peer.Tag;
            SessionComponent playerSession = playerEntity.GetComponent<SessionComponent>();

            Logger.Print($"({playerSession.GetSelectedCharacter().Name}) is moving w/ input: (X:{xAxis}-Y:{yAxis})", Framework.Utils.LogEntryType.Debug);
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
            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.SMSG_REALM_ENTER_WORLD);
            writer.Put(session.GetSelectedCharacter().Id);

            Logger.Print($"Confirmed the player's choice of character: ({session.GetSelectedCharacter().Name})", Framework.Utils.LogEntryType.Debug);
            // todo: [enter world packet] send MOTD, default character speed, etc.

            Global.Network.SendToClient(peer, writer);
        }

        #endregion
    }
}
