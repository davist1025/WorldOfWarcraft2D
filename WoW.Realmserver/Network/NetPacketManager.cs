using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Database.Models.Realm.Character;
using WoW.Framework.Logging;
using WoW.Network;

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

        #endregion

        #region Writers

        /// <summary>
        /// Produces the list of characters that belong to a player using their account Id.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="peer"></param>
        public static void BuildCharacterList(int accountId, NetPeer peer)
        {
            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.SMSG_REALM_CHARACTER_LIST);

            using (var ctx = new RealmContext())
            {
                PlayerCharacter[] thisAccountCharacters = ctx.Characters.Where(character => character.AccountId == accountId).ToArray();
                var characterCount = thisAccountCharacters.Length;

                writer.Put(characterCount);

                for (int i = 0; i < characterCount; i++)
                {
                    var thisCharacter = thisAccountCharacters[i];

                    writer.Put(thisCharacter.Name);
                    writer.Put(thisCharacter.CharacterId);
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

        #endregion
    }
}
