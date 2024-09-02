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
    /// Sent after the client chooses a character to play on.
    /// </summary>
    public class ClientRealm_TransferWorld
    {
        public int LocalCharacterId { get; set; }

    }
}
