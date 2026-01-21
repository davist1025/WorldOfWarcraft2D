using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Network.Objects
{
    public class AccountMetadataObject
    {
        public int Id { get; init; }
        public string SessionId { get; init; }
        public AccountSecurityType SecurityLevel { get; init; }

        public AccountMetadataObject(int id, string sessionId, AccountSecurityType securityLevel)
        {
            Id = id;
            SessionId = sessionId;
            SecurityLevel = securityLevel;
        }
    }
}
