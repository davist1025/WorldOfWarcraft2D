using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Realm;

namespace WoW.Realmserver.Components.Behavior.Samples
{
    [Behavior("basic_mob_passive")]
    public class PassiveMobBehavior : IUpdateableBehavior
    {
        private NpcControllerComponent _controller;
        private TiledMapProcessor _processor;

        private bool _isIdle = true;
        private float _idleTimer = 4.5f;
        private float _currentTime = 0f;
        private Vector2 _travelToPoint = Vector2.Zero;
        private Mover _mover;

        public override void OnLoad()
        {
            Console.WriteLine($"My first passive mob script <3");

            _mover = Parent.AddComponent<Mover>();
            _controller = Parent.GetComponent<NpcControllerComponent>();

            // Processor will give us TiledMap data, namely the collision layer for pathfinding.
            // todo: sequence contains no elements!
            //_processor = Program.Scene
            //    .FindComponentsOfType<TiledMapProcessor>()
            //    .Single(processor => processor.Creatures.Contains(Parent));
        }

        public override void Update()
        {
            // todo: create roam behavior!
            /*
             * Roaming
             * 
             * NPCs should generate a random position within the boundaries of their spawn position.
             * AStar will be used to generate a collison-free path to this position and the NPC will begin movement.
             * 
             * In the presence of players and other NPCS, this creature will remain passive until provoked. 
             * 
             * This mob does not have any special abilities and will only perform basic melee combat.
             */

            /*
             * This behavior will need access to its' Entity's Tiled map data for pathfinding, the NPC metadata, including statistics, and a new event handler for when someone attacks this entity.
             * 
             */

            if (_isIdle)
            {
                _currentTime += Program.DeltaTime;
                if (_currentTime >= _idleTimer)
                {
                    _isIdle = false;

                    // spawner component could be null if this NPC was created by a command.
                    // in this case, we can use the npcs' spawn point.
                    if (_controller.SpawnPosition != Vector2.Zero)
                    {
                        // hack: 200 values from the spawn position of the NPC.
                        // this is just bad debug code ignore it for now
                        _travelToPoint = new Vector2(
                            _controller.SpawnPosition.X + Nez.Random.NextFloat(200f),
                            _controller.SpawnPosition.Y + Nez.Random.NextFloat(200));

                    }
                    else
                    {
                        _travelToPoint = new Vector2(_controller.Spawner.Bounds.X + Nez.Random.NextFloat(_controller.Spawner.Bounds.Width), _controller.Spawner.Bounds.Y + Nez.Random.NextFloat(_controller.Spawner.Bounds.Height));
                    }
                    _currentTime = 0f;
                }
            }

            if (_travelToPoint != Vector2.Zero)
            {
                var distance = _travelToPoint - Parent.Position;
                distance.Normalize();

                bool _shouldFollow = (Vector2.Distance(Parent.Position, _travelToPoint) < 1f) ? false : true;

                if (_shouldFollow)
                {
                    // - 15f for a slower NPC movement.
                    var movement = distance * Time.DeltaTime * (Program.Configuration.WorldParameters["global_movement_speed"] - 15f);

                    _mover.CalculateMovement(ref movement, out CollisionResult collisionResult);
                    _mover.ApplyMovement(movement);

                    // hack: temporary call to update the NPC position to clients while testing the movement code on the server
                    Program.SendToAll(new RealmClient_NetPositionInputUpdate()
                    {
                        Id = Parent.Name,
                        ResultX = Parent.Position.X,
                        ResultY = Parent.Position.Y,
                        MovementX = distance.X,
                        MovementY = distance.Y,
                        IsTeleportUpdate = false
                    });
                }
                else
                {
                    _travelToPoint = Vector2.Zero;
                    _isIdle = true;
                }
            }
        }
    }
}
