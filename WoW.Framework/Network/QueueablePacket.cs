using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Framework.Network
{
    /// <summary>
    /// A netdatawriter that can also store a delivery method.
    /// </summary>
    public class QueueablePacket : NetDataWriter
    {
        public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.ReliableOrdered;

        public QueueablePacket(bool autoResize) : base(autoResize) { }
    }
}
