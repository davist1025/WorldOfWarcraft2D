using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// Sent to any/all clients when a disconnection occurs for any reason.
    /// Contains an optional reason.
    /// </summary>
    public class RealmClient_Disconnect
    {
        public string Id { get; set; }

        public DisconnectCode Code { get; set; }

        public string Reason { get; set; }
    }

    public enum DisconnectCode
    {
        Kick = 0,
        Timeout
    }
}
