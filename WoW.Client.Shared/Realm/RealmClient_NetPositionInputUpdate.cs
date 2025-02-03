using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    public class RealmClient_NetPositionInputUpdate
    {
        public string Id { get; set; }

        public float ResultX { get; set; }

        public float ResultY { get; set; }

        public float MovementX { get; set; }

        public float MovementY { get; set; }
        public int Direction { get; set; }

        public bool IsTeleportUpdate { get; set; }
    }
}
