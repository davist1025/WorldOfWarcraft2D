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

        // todo: movementx/y will be necessary to determine animations, etc.
        [Obsolete("Unused as of 12/25/24. Currecntly using ResultX/ResultY.")]
        public float MovementX { get; set; }

        [Obsolete("Unused as of 12/25/24. Currecntly using ResultX/ResultY.")]
        public float MovementY { get; set; }
    }
}
