using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Server.Shared
{
    /// <summary>
    /// Realm -> Auth.
    /// 
    /// Signals that a player has disconnected.
    /// </summary>
    public class RealmAuth_Disconnection
    {
        public int AccountId { get; set; }
    }
}
