using Microsoft.Xna.Framework;
using Nez;
using Nez.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Network;
using WoW.Client.Scenes;
using WoW.Framework.Logging;
using static WoW.Client.Global;

namespace WoW.Client.Utils
{
    /// <summary>
    /// Potentially globally used functions; typically emitted from some location.
    /// </summary>
    public class GameManager : GlobalManager
    {
        /// <summary>
        /// Triggers  whenever local player movement is != Vector2.Zero.
        /// </summary>
        public EventHandler<Vector2> LocalPlayerMoved;

        /// <summary>
        /// Triggers when a new actor has been sent to this client.
        /// </summary>
        public EventHandler<Entity> NewActorRegistered;

        /// <summary>
        /// Triggers when this client gets disconnected from the server, either by kick or otherwise.
        /// </summary>
        public EventHandler Disconnected;

        private Queue<Entity> _newActorQueue = new Queue<Entity>();

        public GameManager(bool subscribeDefaults = true)
        {
            LocalPlayerMoved += OnLocalPlayerMoved;
            NewActorRegistered += OnNewActorRegistered;
            Disconnected += OnDisconnected;
        }

        #region Default subscribers

        public void OnLocalPlayerMoved(object sender, Vector2 input)
        {
            if (Global.PeerState == GameNetworkState.World)
                NetPacketManager.BuildMovementUpdate(input);
        }

        public void OnNewActorRegistered(object sender, Entity entity)
        {
            _newActorQueue.Enqueue(entity);
        }

        public void OnDisconnected(object sender, EventArgs e) 
        {
            Core.StartSceneTransition(new FadeTransition(() => new LogonScene()));
        }
        #endregion

        public bool PopUntrackedActor(out Entity entity)
        {
            if (_newActorQueue.TryDequeue(out var result))
            {
                entity = result;
                return true;
            }

            entity = null;
            return false;
        }
    }
}
