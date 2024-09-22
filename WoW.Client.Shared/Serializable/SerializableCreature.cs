using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Serializable
{
    /// <summary>
    /// Describes a creature for the network.
    /// </summary>
    public class SerializableCreature
    {
        public string Name { get; set; }
        public string SubName { get; set; }
        public string DisplayId { get; set; }
        public GameObjectFlags Flags { get; set; }
    }
}
