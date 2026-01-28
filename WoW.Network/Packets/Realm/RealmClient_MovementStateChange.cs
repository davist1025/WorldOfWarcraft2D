using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Packets.Realm
{
    public class RealmClient_MovementStateChange
    {
        public string Id { get; set; }

        [Obsolete("TickId is unused.")]
        public int TickId { get; set; }

        public float ResultX { get; set; }

        public float ResultY { get; set; }

        public bool IsColliding { get; set; }
        public Vector2Serializable ColliderNormal { get; set; }
        public float MovementX { get; set; }

        public float MovementY { get; set; }
        public int Direction { get; set; }

        public bool IsTeleportUpdate { get; set; }
    }
}
