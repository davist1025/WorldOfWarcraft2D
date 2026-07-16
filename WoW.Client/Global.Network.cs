using LiteNetLib;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Network;
using WoW.Network.Objects;

namespace WoW.Client
{
    public partial class Global
    {
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
        public static RealmserverMetadataObject LastRealm;


        public static List<RealmserverMetadataObject> Realmlist = new List<RealmserverMetadataObject>();
        public static List<CharacterMetadataObject> Characters = new List<CharacterMetadataObject>();

        /// <summary>
        /// Is set by the game server.
        /// todo: [player speed] create a component for this.
        /// </summary>
        public static float Speed = 1f;
    }
}
