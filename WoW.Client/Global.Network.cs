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
        public static Entity Player;
        public static string Username;
        public static string SessionId;
        public static RealmserverMetadataObject LastRealm;

        public static NetworkController Network;
        public static GameNetworkState OnlineState = GameNetworkState.Offline;

        /// <summary>
        /// Is set by the game server.
        /// </summary>
        public static float Speed = 1f;
    }
}
