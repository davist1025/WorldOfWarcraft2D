using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network.Packets.Realm
{
    public class RealmClient_EnterWorld
    {
        public float MovementSpeed { get; set; }
        public string MOTD { get; set; }
    }
}
