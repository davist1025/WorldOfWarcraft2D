using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Client
{
    /// <summary>
    /// Client -> Auth.
    /// 
    /// Contains logon information.
    /// </summary>
    public class ClientAuth_Logon
    {
        public string AccountName { get; set; }

        /* todo: client auth
         * password (what kind of encryption?)
         * game client version/build
         * ???
         */
    }
}
