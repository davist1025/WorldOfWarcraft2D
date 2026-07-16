using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Network;

namespace WoW.Realmserver
{
    /// <summary>
    /// A container for globally accessed objects.
    /// </summary>
    internal partial class Global
    {
        public static NetworkManager Network;
    }
}
