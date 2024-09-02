using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Auth
{
    /// <summary>
    /// Auth -> Client
    /// 
    /// Only appears after the client has a successful logon.
    /// </summary>
    public class AuthClient_Logon
    {
        public string SessionId { get; set; }
    }
}
