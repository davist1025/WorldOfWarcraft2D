using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Client
{
    /// <summary>
    /// Client -> Realms
    /// Sent whenever the movement key inputs change on the client.
    /// Values will be 1 or -1 and 0.
    /// </summary>
    public class ClientRealm_Movement
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Tick { get; set; }
    }
}
