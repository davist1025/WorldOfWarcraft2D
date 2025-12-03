using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components
{
    // todo: extend this to work with resources like ore
    /// <summary>
    /// Spawns an NPC into the game world on a timer.
    /// </summary>
    public class SpawnerComponent : Component, IUpdatable
    {
        private TiledMapProcessor _processor;

        private int _npcId = -1;
        private int _maxInWorld = 0;
        private int _currentlyInWorld = 0;
        private float _timerInSeconds = 0f;
        private float _localTimer = 0f;

        private RectangleF _bounds;
        public RectangleF Bounds
        {
            get => _bounds;
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
        }

        private bool _isPlayerSpawner = false;
        public bool IsPlayerSpawner
        {
            get => _isPlayerSpawner;
        }

        public SpawnerComponent(TiledMapProcessor processor, int npcId, int macCount, float timerInSeconds, bool isPlayerSpawner, Vector2 position, Vector2 size)
        {
            _processor = processor;
            _position = position;

            if (!isPlayerSpawner)
            {
                _npcId = npcId;
                _maxInWorld = macCount;
                _timerInSeconds = timerInSeconds;
                _bounds = new RectangleF(new Vector2(position.X - size.X / 2f, position.Y - size.Y / 2f), size);
            }

            _isPlayerSpawner = isPlayerSpawner;
        }

        public override void OnAddedToEntity()
        {
        }

        public void Update()
        {
            if (!_isPlayerSpawner && _currentlyInWorld < _maxInWorld)
            {
                _localTimer += Time.DeltaTime;

                if (_localTimer >= _timerInSeconds + Nez.Random.NextFloat(5f))
                {
                    _localTimer = 0f;

                    var newRandomPosition = new Vector2(_bounds.X + Nez.Random.NextFloat(_bounds.X + _bounds.Width), _bounds.Y + Nez.Random.NextFloat(_bounds.Height));

                    var npc = EntityFactory.CreateNPC(_npcId, _processor.Map.Properties["id"], _position);
                    npc.GetComponent<NpcControllerComponent>().SetHome(this);

                    _currentlyInWorld++;
                }
            }
        }
    }
}
