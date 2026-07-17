using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Network;

namespace WoW.Realmserver
{
    /// <summary>
    /// A container for globally accessed objects.
    /// </summary>
    internal partial class Global
    {
        public static NetworkManager Network;

        /// <summary>
        /// A queue of players transferring from the authserver.
        /// </summary>
        public static Queue<Tuple<string, NetPeer>> Transfers = new Queue<Tuple<string, NetPeer>>();
    }
}
