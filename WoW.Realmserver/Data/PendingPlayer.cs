using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Data
{
    /// <summary>
    /// Represents a player that has just authenticated with the Authserver, now attempting to connect to a realmserver.
    /// </summary>
    public class PendingPlayer
    {
        public string SessionId { get; init; }
        public NetPeer Connection { get; init; }

        public PendingPlayer(string sessionId, NetPeer connection) {  SessionId = sessionId; Connection = connection; }
    }
}
