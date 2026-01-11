using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Client.Packets.Client
{
    /// <summary>
    /// Client -> Realm.
    /// Requests that a new character be made.
    /// </summary>
    public class ClientRealm_CreateCharacter
    {
        public string Name { get; set; }
        public int RaceId { get; set; }
        public int HairId { get; set; }
    }
}
