using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Client.Packets.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// Returns a status code for the player.
    /// This status code will also dictate what UI to display/what packets to send.
    /// </summary>
    public class RealmClient_CreateCharacter
    {
        public enum Result
        {
            Success = 0,
            NameInUse,
            NameBanned,
        }

        public Result CreationResult { get; set; }
    }
}
