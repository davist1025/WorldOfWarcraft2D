using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Client
{
    /// <summary>
    /// Client -> Realm.
    /// Requests that a new character be made.
    /// </summary>
    public class ClientRealm_CreateCharacter
    {
        public string Name { get; set; }
        public int RaceId { get; set; }
    }
}
