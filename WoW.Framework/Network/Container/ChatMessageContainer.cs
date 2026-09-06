using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Framework.Network.Container
{
    /// <summary>
    /// Describes a chat message object on the Client for easy storage.
    /// </summary>
    public class ChatMessageContainer
    {
        public string Input { get; set; }
    }
}
