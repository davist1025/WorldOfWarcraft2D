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
    /// 
    /// todo: CharacterContainer might be unnecessary.
    /// This was intended to be used by both the client and realmserver, but so far is only used by the client.
    /// </summary>
    public class CharacterContainer
    {
        public string Name { get; init; }
        public int Id { get; init; }
        public ActorRaceType RaceId { get; init; }
        public int HairId { get; init; }
        public string MapId { get; init; }
        public ActorAnimationDirection Direction { get; init; }

        public CharacterContainer(string name, int id, ActorRaceType raceId, int hairId, string mapId, ActorAnimationDirection direction)
        {
            Name = name;
            Id = id;
            RaceId = raceId;
            HairId = hairId;
            MapId = mapId;
            Direction = direction;
        }
    }
}
