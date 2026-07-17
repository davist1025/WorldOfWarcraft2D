using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;

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
    }
}
