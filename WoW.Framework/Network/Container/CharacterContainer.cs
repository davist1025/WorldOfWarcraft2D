using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Framework.Network.Container
{
    /// <summary>
    /// Describes a player character for the Client for easy storage.
    /// </summary>
    public class CharacterContainer
    {
        public string Name { get; init; }
        public int Id { get; init; }
        public ActorRaceType RaceId { get; init; }
        public int HairId { get; init; }
        public string MapId { get; init; }
        public float X { get; init; }
        public float Y { get; init; }
        public ActorAnimationDirection Direction { get; init; }

        public CharacterContainer(string name, int id, ActorRaceType raceId, int hairId, string mapId, float x, float y, ActorAnimationDirection direction)
        {
            Name = name;
            Id = id;
            RaceId = raceId;
            HairId = hairId;
            MapId = mapId;
            X = x;
            Y = y;
            Direction = direction;
        }
    }
}
