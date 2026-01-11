using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Client.Packets.Client
{
    /// <summary>
    /// Client -> Realms
    /// Sent whenever the movement key inputs change on the client.
    /// Values will be 1 or -1 and 0.
    /// </summary>
    public class ClientRealm_Movement
    {
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public long Sequence { get; set; }
        public float DeltaTime { get; set; }
    }
}
