using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework;
using static WoW.Framework.Utils;

namespace WoW.Network.Objects
{
    /// <summary>
    /// Shared between the client and server that describes an individual NPC based on existing default data.
    /// </summary>
    public class NpcMetadataObject
    {
        public string Uid { get; set; }
        public string Name { get; set; }

        [Obsolete("Unused ModelId for NPC metadata.")]
        public string ModelId { get; set; }

        public ActorFlagTypes Flags { get; set; }
        public int Level { get; set; }
        public string MapId { get; set; }
        public Vector2S Position { get; set; }
    }
}
