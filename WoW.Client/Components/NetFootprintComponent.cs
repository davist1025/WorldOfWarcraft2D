using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Components
{
    /// <summary>
    /// Holds persistent information strictly related to our "footprint," within the server.
    /// 
    /// This includes SessionId, Account Name, etc.
    /// </summary>
    internal class NetFootprintComponent : Component
    {
        /// <summary>
        /// The client's account name.
        /// </summary>
        public string AccountName { get; private set; }

        /// <summary>
        /// The client's unique session id.
        /// </summary>
        public string SessionId { get; private set; }

        public void Create(string accountName, string sessionId)
        {
            AccountName = accountName;
            SessionId = sessionId;
        }
    }
}
