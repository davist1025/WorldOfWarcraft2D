using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// 
    /// Signals to create a networked player object. 'MapId' is omitted here because only players within the same MapId need to know about eachother, and friends list.
    /// </summary>
    public class RealmClient_CreateNetPlayer
    {
        public string WorldId { get; set; }

        /// <summary>
        /// The unique character name for this player.
        /// </summary>
        public string Name { get; set; }
        public int RaceId { get; set; }
        public int HairId { get; set; }

        public float ZoneX { get; set; }
        public float ZoneY { get; set; }
    }
}
