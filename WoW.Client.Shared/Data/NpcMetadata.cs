using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Data
{
    public class NpcMetadata
    {
        public string WorldId { get; set; }
        public string Name { get; set; }
        public string ModelId { get; set; }
        public int Flags { get; set; }
        public int Level { get; set; }

        public string MapId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public NpcTypeFlags FlagBits
        {
            get
            {
                return (NpcTypeFlags)Flags;
            }
        }
    }
}
