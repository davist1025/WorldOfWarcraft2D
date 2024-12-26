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

        [Obsolete("Unused server position update result X.")]
        public float ResultX { get; set; }

        [Obsolete("Unused server position update result Y.")]
        public float ResultY { get; set; }

        public float MovementX { get; set; }

        public float MovementY { get; set; }
    }
}
