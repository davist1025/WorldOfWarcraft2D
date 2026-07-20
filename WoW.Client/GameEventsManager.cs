using Nez.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static WoW.Client.Global;

namespace WoW.Client
{
    /// <summary>
    /// Potentially globally used functions; typically emitted from some location.
    /// </summary>
    internal class GameEventsManager
    {
        /// <summary>
        /// Emitted from the client when the local player moves.
        /// </summary>
        /// <param name="resultPosition"></param>
        public static void OnPlayerMove(Vector2 resultPosition)
        {

        }
    }
}
