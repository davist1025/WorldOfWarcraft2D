using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Server.Shared
{
    /// <summary>
    /// Realm -> Authserver.
    /// 
    /// Requests verification on the given session id.
    /// </summary>
    public class RealmAuth_SessionVerification
    {
        public string SessionId { get; set; }
    }
}
