using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;
using static WoW.Framework.Utils;

namespace WoW.Client.Components
{
    /// <summary>
    /// Contains the networked metadata with information regarding this player.
    /// </summary>
    public class OnlinePlayerData
    {
        public readonly int NetworkId;
        public readonly string CharacterName;
        public readonly int HairId;
        public readonly ActorRaceType Race;
        public string MapId;
        public float X;
        public float Y;

        public OnlinePlayerData(int networkId, string name, int hairId, int raceType, string mapId, float x, float y)
        {
            NetworkId = networkId;
            CharacterName = name;
            HairId = hairId;
            Race = (ActorRaceType)raceType;
            MapId = mapId;
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// The controller for online players.
    /// </summary>
    public class OnlinePlayerControllerComponent : Component, IUpdatable
    {
        public OnlinePlayerData Data { get; init; }

        private Vector2 _moveDirection = Vector2.Zero;
        private SubpixelVector2 _subPixelMovement;
        private Mover _mover;
        private CircleCollider _circleCollider;

        private Queue<Vector2> _movementUpdates = new Queue<Vector2>();

        public OnlinePlayerControllerComponent(OnlinePlayerData metadata)
        {
            Data = metadata;
        }

        public override void OnAddedToEntity()
        {
            Entity.SetPosition(new Vector2(Data.X, Data.Y));

            _mover = Entity.AddComponent<Mover>();
            // todo: collider.

        }

        public void Update()
        {
            if (_movementUpdates.TryDequeue(out Vector2 input))
            {
                Logger.Print($"'{Data.CharacterName}' is moving!", LogEntryType.Debug);
            }
        }

        public void EnqueuePositionChange(Vector2 input) => _movementUpdates.Enqueue(input);
    }
}
