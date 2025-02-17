using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Server.Shared
{
    public class Vocab
    {
        public enum SecurityLevel
        {
            Player = 1, // has access to ALL commands and server functionality.
            Gamemaster,
            Administrator
        }
    }
}
