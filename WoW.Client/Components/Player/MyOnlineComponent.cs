using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Network.Container;

namespace WoW.Client.Components.Player
{
    /// <summary>
    /// Holds persistent information strictly related to our "footprint," within the server.
    /// 
    /// This includes SessionId, Account Name, characters, etc,.
    /// </summary>
    internal class MyOnlineComponent : Component
    {
        /// <summary>
        /// The client's account name.
        /// </summary>
        public string AccountName { get; init; }

        /// <summary>
        /// The client's unique session id.
        /// </summary>
        public string SessionId { get; init; }

        public List<CharacterContainer> Characters { get; private set; }
        private int _selectedCharacter = -1;

        public MyOnlineComponent(string accountName, string sessionId)
        {
            AccountName = accountName;
            SessionId = sessionId;
        }

        public void SetCharacterList(List<CharacterContainer> characters)
        {
            Characters = characters;
        }

        public void SetSelectedCharacter(int index)
        {
            _selectedCharacter = index;
        }

        public CharacterContainer GetSelectedCharacter() => Characters[_selectedCharacter];
    }
}
