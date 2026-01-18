using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Network.Packets.Authenticcation
{
    public class AuthClient_LogonCode
    {
        public AuthCodeType Code { get; set; }
    }
}
