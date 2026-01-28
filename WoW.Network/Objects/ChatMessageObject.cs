using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Network.Objects
{
    /// <summary>
    /// Shared object between the client and server that describes a chat message.
    /// </summary>
    public class ChatMessageObject
    {
        public string Input { get; set; }
        public ChatChannelType Channel { get; set; }
    }
}
