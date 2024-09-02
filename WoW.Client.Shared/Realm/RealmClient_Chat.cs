using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client
    /// 
    /// Sent to all clients when someone sends a chat message.
    /// This packet is also used for whispering, group chatting, etc.
    /// </summary>
    public class RealmClient_Chat
    {
        // todo: channel

        public string Id { get; set; }
        public string Message { get; set; }
    }
}
