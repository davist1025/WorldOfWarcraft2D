using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Data
{
    /// <summary>
    /// Describes a Realm sent over the network.
    /// </summary>
    public class RemoteRealmserver
    {
        public string Name { get; init; }
        public string Hostname { get; init; }
        public int Port { get; init; }

        public RemoteRealmserver(string name, string hostname, int port)
        {
            Name = name;
            Hostname = hostname;
            Port = port;
        }
    }
}
