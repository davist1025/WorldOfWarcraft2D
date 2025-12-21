using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Realm;

namespace WoW.Realmserver.Components.Behavior.Default
{
    [Behavior("roam")]
    public class RoamBehavior : IUpdateableBehavior
    {
        private float _timerInSeconds;
        private float _lastTime = 0f;

        private Vector2 _randomPosition = Vector2.Zero;
        private bool _isRoaming = false;
        private AstarGridGraph _graph;
        private List<Point> _pathPoints = new List<Point>();
        private int _pathCount = 0;
        private bool _isMovingToPoint = false;
        private int _nextPointIndex = -1;

        private Point _myTilePos;

        public override void OnLoad()
        {
            UpdateOrder = 1;

            _graph = new AstarGridGraph(Controller.Processor.CollisionLayer);
            _timerInSeconds = 5f;

            Log.Print($"Added Roaming to creature ({Controller.Metadata.Name}:{Controller.Metadata.WorldId})", LogType.Debug);
        }

        public override void Update()
        {
            if (!_isRoaming)
                _lastTime += Time.DeltaTime;

            if (_lastTime >= _timerInSeconds)
            {
                _lastTime = 0f;
                _isRoaming = true;

                _randomPosition = new Vector2(
                    Nez.Random.Range(Controller.Spawner.Bounds.X, Controller.Spawner.Bounds.X + Controller.Spawner.Bounds.Width), 
                    Nez.Random.Range(Controller.Spawner.Bounds.Y, Controller.Spawner.Bounds.Y + Controller.Spawner.Bounds.Height));
            }

            if (_isRoaming && _randomPosition != Vector2.Zero && _pathPoints != null && _pathPoints.Count == 0)
            {
                _myTilePos = Controller.Processor.Map.WorldToTilePosition(Controller.Entity.Position);
                var targetPosition = Controller.Processor.Map.WorldToTilePosition(_randomPosition);

                _pathPoints = _graph.Search(_myTilePos, targetPosition);

                if (_pathPoints == null)
                {
                    Log.Print($"{GetType().Name}: _pathPoints was null after attempting to search for a path to: {targetPosition}; resetting...", LogType.Debug);
                    _isRoaming = false;
                    _randomPosition = Vector2.Zero;
                }
                else
                    _pathCount = _pathPoints.Count;
            }

            if (!_isMovingToPoint && _pathPoints != null)
            {
                if (_nextPointIndex == _pathCount - 1 || _pathPoints.Count == 0)
                {
                    _isRoaming = false;
                    _randomPosition = Vector2.Zero;
                    _pathPoints.Clear();
                    _nextPointIndex = -1;
                }
                else
                {
                    _isMovingToPoint = true;
                    _nextPointIndex++;
                }
            }

            if (_isMovingToPoint)
            {
                var nextPoint = Controller.Processor.Map.TileToWorldPosition(_pathPoints[_nextPointIndex]);
                
                var direction = nextPoint - Controller.Entity.Position;
                var dist = direction.Length();
                direction = (dist > 0.000001f) ? (direction /= dist) : Vector2.Zero;
                // credits: https://stackoverflow.com/questions/60932940/vector2-normalizeendposition-startposition-is-nan-what-can-i-do

                Controller.Mover.Move(new Vector2(1f) * direction, out var _);

                Program.SendToAll(new RealmClient_MovementStateChange()
                {
                    // todo: send simulated direction so the client can replicate animations?
                    Id = Controller.Metadata.WorldId,
                    ResultX = Controller.Entity.Position.X,
                    ResultY = Controller.Entity.Position.Y
                });

                if (Vector2.Distance(Controller.Entity.Position, nextPoint) <= 1.0f)
                {
                    _isMovingToPoint = false;
                }
            }
        }
    }
}
