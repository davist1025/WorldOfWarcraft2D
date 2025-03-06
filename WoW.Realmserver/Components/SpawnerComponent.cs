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
        private float _timerInSeconds = 0f;

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

        public SpawnerComponent(TiledMapProcessor processor, int npcId, int macCount, float timerInSeconds, bool isPlayerSpawner, Vector2 position)
        {
            _processor = processor;
            _position = position;

            if (!isPlayerSpawner)
            {
                _npcId = npcId;
                _maxInWorld = macCount;
                _timerInSeconds = timerInSeconds;
            }

            _isPlayerSpawner = isPlayerSpawner;
        }

        public override void OnAddedToEntity()
        {
            // todo: get npc metadata from DB.
        }

        public void Update()
        {

        }
    }
}
