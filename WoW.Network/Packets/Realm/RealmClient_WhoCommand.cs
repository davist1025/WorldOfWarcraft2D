using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Packets.Realm
{
    /// <summary>
    /// Realm -> Client
    /// 
    /// Sends a detailed list of all online players.
    /// </summary>
    public class RealmClient_WhoCommand
    {
        public string[] Characters { get; set; }
    }
}
