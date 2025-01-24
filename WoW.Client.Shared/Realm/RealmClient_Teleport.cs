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
    /// Teleports the character to the given position.
    /// 
    /// This packet is sent to all players within the given Characters' MapId so they can properly remove the entity/renderer.
    /// </summary>
    public class RealmClient_Teleport
    {
        /// <summary>
        /// The character to teleport.
        /// </summary>
        public string WorldId { get; set; }

        public string MapId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
