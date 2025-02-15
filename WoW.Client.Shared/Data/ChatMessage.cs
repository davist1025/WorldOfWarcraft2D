using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Data
{
    public class ChatMessage
    {
        public string Message { get; set; }

        public ChatMessageFlag Flags { get; set; }
    }
}
