using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework;
using WoW.Framework.Logging;

namespace WoW.Authserver
{
    /// <summary>
    /// Handles reading and writing packet data.
    /// </summary>
    internal class NetPacketManager
    {
        #region Readers

        public static void ReadLogin(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            Logger.Print($"Handling logon packet from: {peer.EndPoint}.", Utils.LogEntryType.Debug);
        }

        #endregion
    }
}
