using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// 
    /// Sends a copy of the server's player position to the player.
    /// </summary>
    public class RealmClient_Debug_ServerPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
