using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Client.Packets.Client
{
    /// <summary>
    /// Client -> Realm.
    /// 
    /// Signals the realm to delete a character.
    /// </summary>
    public class ClientRealm_DeleteCharacter
    {
        public int CharacterId { get; set; }
    }
}
