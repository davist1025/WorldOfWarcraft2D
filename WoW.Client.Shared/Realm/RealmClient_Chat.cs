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
        public bool IsWhisper { get; set; }

        public string FromWorldId { get; set; } // can be an NPCs WorldId, a player's character name, etc.
        public string Message { get; set; }
    }
}
