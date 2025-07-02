using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.Pathfinding;
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
        private bool _isMoving = false;
        private float _idleTimer = 4.5f;
        private float _currentTime = 0f;
        private Vector2 _travelToPoint = Vector2.Zero;
        private Mover _mover;

        private AstarGridGraph _aStarGrid;
        private List<Point> _graphTravelPoints;
        private Vector2 _travelToSpace = Vector2.Zero;

        public override void OnLoad()
        {
            Console.WriteLine($"My first passive mob script <3");

            _mover = Parent.AddComponent<Mover>();
            _controller = Parent.GetComponent<NpcControllerComponent>();

            // Processor will give us TiledMap data, namely the collision layer for pathfinding.

            var allProcessors = Program.Scene.FindComponentsOfType<TiledMapProcessor>();
            var processor = allProcessors.Single(processor => processor.Creatures.Any(entity => entity.Id == Parent.Id));

            if (processor != null)
            {
                Console.WriteLine($"Found processor which contains this enetity: {processor.Map.Properties["id"]}");
                _aStarGrid = new AstarGridGraph(processor.CollisionLayer);

                _processor = processor;
            }
        }

        public override void Update()
        {
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

                    /*
                     * A complete path from the parent's position, to a randomly generated position within their spawn radius needs to be calculated using AStar.
                     * 
                     * We'll need:
                     * - The parent's position
                     * - A randomly generated end goal
                     * 
                     */

                    var worldToTiles = _processor.Map.WorldToTilePosition(_controller.Spawner.Position);
                    var generatedX = Nez.Random.Range(worldToTiles.X - 5f, worldToTiles.X + 5f);
                    var generatedY = Nez.Random.Range(worldToTiles.Y - 5f, worldToTiles.Y + 5f);

                    _graphTravelPoints = _aStarGrid
                        .Search(
                            _processor.Map.WorldToTilePosition(_controller.Entity.Position), new Point((int)generatedX, (int)generatedY));


                    if (_graphTravelPoints != null)
                        Debug.Log("Generated movement points for NPC...");

                    _currentTime = 0f;
                }
            }

            // select the next point in the node graph.
            if (_graphTravelPoints != null)
            {
                if (_graphTravelPoints.Count > 0 && !_isMoving)
                {
                    _isMoving = true;

                    var point = _graphTravelPoints.First();
                    _graphTravelPoints.Remove(point);
                    _travelToSpace = _processor.Map.TileToWorldPosition(point);
                }
            }

            // travel to the next tile's world position in the node graph.
            if (_travelToSpace != Vector2.Zero)
            {
                bool _shouldFollow = (Vector2.Distance(Parent.Position, _travelToSpace) < 1f) ? false : true;

                if (_shouldFollow)
                {
                    var distance = _travelToSpace - Parent.Position;
                    distance.Normalize();

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
                    _isMoving = false;
                    _travelToSpace = Vector2.Zero;
                }
            }

            // reset the idle and node graph if we've reached the end of the path.
            if (_graphTravelPoints != null && _graphTravelPoints.Count == 0)
            {
                _graphTravelPoints = null;
                _isIdle = true;
            }

            if (_graphTravelPoints == null && !_isIdle)
                _isIdle = true;
        }
    }
}
