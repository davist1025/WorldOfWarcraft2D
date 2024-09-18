using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// Tells the player to create a new GameObject that can have flags, which tell the player how to handle them.
    /// </summary>
    public class RealmClient_CreateNetObject
    {
        public string Id { get; set; }

        /// <summary>
        /// The image or animation used to render this object.
        /// </summary>
        public string DisplayId { get; set; }

        public GameObjectFlags Flags => (GameObjectFlags)FlagsLiteral;

        public int FlagsLiteral = 0;
    }
}
