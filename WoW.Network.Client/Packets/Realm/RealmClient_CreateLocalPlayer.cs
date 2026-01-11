using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Client.Packets.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// </summary>
    public class RealmClient_CreateLocalPlayer
    {
        public string WorldId { get; set; }
        public string Name { get; set; }
        public int RaceId { get; set; }
        public int HairId { get; set; }
        public float ZoneX { get; set; }
        public float ZoneY { get; set; }
        public int Direction { get; set; }

        /// <summary>
        /// Used client-side to load the given TiledMap.
        /// </summary>
        public string MapId { get; set; }

        // todo: statistics and attributes.
    }
}
