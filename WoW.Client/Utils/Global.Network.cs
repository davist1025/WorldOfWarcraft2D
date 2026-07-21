using LiteNetLib;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Network;
using WoW.Framework.Network.Container;

namespace WoW.Client
{
    public partial class Global
    {
        public enum GameNetworkState
        {
            Offline,
            Offline_Realm,
            Offline_Characters,
            Offline_World,
            Auth_LoggingIn,
            Auth_Banned,
            Auth_Invalid,
            Auth_IsOnline,
            Auth_Realmlist,
            Realm,
            Realm_Characters,
            Realm_CreateCharacter,
            Realm_CharacterNameInvalid,
            LoadingWorld,
            World
        }

        /// <summary>
        /// The network peer. 
        /// 
        /// This is set upon connecting to a server successfully.
        /// </summary>
        public static NetPeer Peer = null;
        public static NetworkManager Network;
        public static GameNetworkState PeerState = GameNetworkState.Offline;

        public static Entity Player;
        public static string Username;
        public static string SessionId;


        public static List<RealmserverContainer> Realmlist = new List<RealmserverContainer>();
        public static List<CharacterContainer> Characters = new List<CharacterContainer>();
        private static int _selectedCharacterIndex = -1;

        /// <summary>
        /// Is set by the game server.
        /// todo: [player speed] create a component for this.
        /// </summary>
        public static float Speed = 1f;

        public static void SetSelectedCharacter(int index)
            => _selectedCharacterIndex = index;

        public static CharacterContainer GetSelectedCharacter()
            => Characters[_selectedCharacterIndex];
    }
}
