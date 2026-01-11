using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Client.Packets.Realm
{
    /// <summary>
    /// Sets the client's current target.
    /// </summary>
    public class RealmClient_SetTarget
    {
        public string WorldId { get; set; }
    }
}
