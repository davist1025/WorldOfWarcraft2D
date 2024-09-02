using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Client
{
    /// <summary>
    /// Client -> Realm.
    /// 
    /// Request a connection to the realmserver by sending our session id.
    /// </summary>
    public class ClientRealm_TransferLogon
    {
        public string SessionId { get; set; }
    }
}
