using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Network.Objects
{
    /// <summary>
    /// Utilized when a player requests their character list or sends a new potential character to the server.
    /// </summary>
    public class CharacterMetadataObject
    {
        /// <summary>
        /// May be null if this object is built by the client.
        /// </summary>
        public string Id { get; init; } = null;
        public string Name { get; init; }
        public ActorRaceType Race { get; init; }
        public int Hair { get; init; }

        /// <summary>
        /// May be null if this object is built by the client.
        /// </summary>
        public string MapId { get; init; } = null;

        public CharacterMetadataObject(string name, ActorRaceType race, int hair, string id = null, string mapId = null)
        {
            Id = id;
            Name = name;
            Race = race;
            Hair = hair;
            MapId = mapId;
        }
    }
}
