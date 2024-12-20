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

        public RemoteCharacter(int characterId, string characterName)
        {
            CharacterId = characterId;
            CharacterName = characterName;
        }
    }
}
