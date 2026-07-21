using Microsoft.Xna.Framework;
using Nez;
using Nez.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Network;
using WoW.Framework.Logging;
using static WoW.Client.Global;

namespace WoW.Client.Utils
{
    /// <summary>
    /// Potentially globally used functions; typically emitted from some location.
    /// </summary>
    public class GameEventsManager : GlobalManager
    {
        /// <summary>
        /// Triggers it's subscription(s) whenever local player movement is != Vector2.Zero.
        /// </summary>
        public EventHandler<Vector2> LocalPlayerMoved;

        public GameEventsManager(bool subscribeDefaults = true)
        {
            LocalPlayerMoved += OnLocalPlayerMoved;
        }

        #region Default subscribers

        public void OnLocalPlayerMoved(object sender, Vector2 input)
        {
            if (Global.PeerState == GameNetworkState.World)
                NetPacketManager.BuildMovementUpdate(input);
        }

        #endregion
    }
}
