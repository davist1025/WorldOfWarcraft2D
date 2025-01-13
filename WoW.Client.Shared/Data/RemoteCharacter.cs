using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Data
{
    public class RemoteCharacter
    {
        public int CharacterId { get; init; }
        public string CharacterName { get; init; }
        public int RaceId { get; init; }

        public RemoteCharacter(int characterId, string characterName, int raceId)
        {
            CharacterId = characterId;
            CharacterName = characterName;
            RaceId = raceId;
        }
    }
}
