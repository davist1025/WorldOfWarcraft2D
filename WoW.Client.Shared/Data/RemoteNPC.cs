using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Data
{
    public class RemoteNPC
    {
        public string Name { get; set; }
        public int Flags { get; set; }
        public int Level { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
    }
}
