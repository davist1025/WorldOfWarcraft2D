using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// Tells the player to create a new object that can have flags.
    /// </summary>
    public class RealmClient_ObjectCreate
    {
        public string Id { get; set; }

        // todo: unused display id.
        // the client will have resources to translate this into an image/sprite/animation.
        public string DisplayId { get; set; }

        public ObjectFlags Flags { get; set; }
    }
}
