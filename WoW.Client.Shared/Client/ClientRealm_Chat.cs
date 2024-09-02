using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Client
{
    /// <summary>
    /// Client <-> Realm.
    /// 
    /// Sent to the realmserver when the client wants to chat, regardless of channel.
    /// </summary>
    public class ClientRealm_Chat
    {
        public string Message { get; set; }
    }
}
