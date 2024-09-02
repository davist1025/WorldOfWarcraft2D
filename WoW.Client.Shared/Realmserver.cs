using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    public class Realmserver
    {
        public string Name { get; init; }
        public string Ip { get; init; }
        public int Port { get; init; }

        public Realmserver(string name, string ip, int port)
        {
            Name = name;
            Ip = ip;
            Port = port;
        }
    }
}
