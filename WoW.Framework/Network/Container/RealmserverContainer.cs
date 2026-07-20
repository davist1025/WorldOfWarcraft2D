using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Framework.Network.Container
{
    /// <summary>
    /// Describes a realmlist object on the Client for easy storage.
    /// </summary>
    public class RealmserverContainer
    {
        public string Name { get; init; }
        public string Hostname { get; init; }
        public int Port { get; init; }

        // todo: [realmserver container] add flags.

        public RealmserverContainer(string name, string hostname, int port)
        {
            Name = name;
            Hostname = hostname;
            Port = port;
        }
    }
}
