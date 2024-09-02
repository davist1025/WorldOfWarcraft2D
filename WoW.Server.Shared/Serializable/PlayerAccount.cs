using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Server.Shared.Vocab;

namespace WoW.Server.Shared.Serializable
{
    /// <summary>
    /// Shared between the auth and realmserver.
    /// 
    /// Describes a user.
    /// </summary>
    public class PlayerAccount
    {
        public int Id;
        public string SessionId;
        public SecurityLevel Security;
    }
}
