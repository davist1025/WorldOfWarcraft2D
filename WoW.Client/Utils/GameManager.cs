using Microsoft.Xna.Framework;
using Nez;
using Nez.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components.Player;
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
        private long _movementTimeTick;
        private List<(long Tick, Vector2 Input, Vector2 ResultingClientPosition)> _movementTickChanges;

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

            _movementTickChanges = new List<(long tick, Vector2 input, Vector2 resultingPosition)>();
        }

        #region Default subscribers

        /// <summary>
        /// Triggered when the local player moves.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="input"></param>
        public void OnLocalPlayerMoved(object sender, Vector2 input)
        {
            if (Global.PeerState == GameNetworkState.World)
            {
                Entity myPlayer = Core.Scene.FindComponentOfType<MyPlayerControllerComponent>().Entity;
                long movementTimeTick = DateTime.Now.Ticks; // used to track individual input during reconciliation.

                _movementTickChanges.Add(new(movementTimeTick, input, myPlayer.Position)); // stores relevant movement information at the time of processing so when the server sends us our real calcuation, we can cross-check and reconcile.

                NetPacketManager.BuildMovementUpdate(input, movementTimeTick);
            }
        }

        /// <summary>
        /// Queues an Actor for processing that was created by the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entity"></param>
        public void OnNewActorRegistered(object sender, Entity entity)
        {
            _newActorQueue.Enqueue(entity);
        }

        /// <summary>
        /// Handles a disconnection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete("Unused still as of 8/9.")]
        public void OnDisconnected(object sender, EventArgs e) 
        {
            Core.StartSceneTransition(new FadeTransition(() => new LogonScene()));
        }
        #endregion

        /// <summary>
        /// Pops an Actor from the queue that was created by the server and begins processing it.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Attempts to locate a movement change by it's <see cref="DateTime.Now"/> value in Ticks (long).
        /// </summary>
        /// <param name="timeTick"></param>
        /// <returns></returns>
        public Tuple<long, Vector2, Vector2> FindMovementChangeByTick(long timeTick) => _movementTickChanges.Find((tuple) => tuple.Tick == timeTick).ToTuple();
    }
}
