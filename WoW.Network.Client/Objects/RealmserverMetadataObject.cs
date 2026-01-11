using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Objects
{
    /// <summary>
    /// Describes a Realmserver for the client.
    /// </summary>
    public class RealmserverMetadataObject
    {
        public string Name { get; init; }
        public string Hostname { get; init; }
        public int Port { get; init; }

        // todo: realm flags.
    }
}
