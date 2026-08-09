using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;
using WoW.Framework.Shared.Components;
using static WoW.Framework.Utils;

namespace WoW.Client.Components.Player
{
    /// <summary>
    /// Contains the networked metadata with information regarding this player.
    /// </summary>
    public class OnlinePlayerData
    {
        public readonly string NetworkId;
        public readonly string CharacterName;
        public readonly int HairId;
        public readonly ActorRaceType Race;
        public string MapId;
        public float X;
        public float Y;
        public float MovementSpeed;

        public OnlinePlayerData(string networkId, string name, int hairId, int raceType, string mapId, float x, float y, float mvoementSpeed)
        {
            NetworkId = networkId;
            CharacterName = name;
            HairId = hairId;
            Race = (ActorRaceType)raceType;
            MapId = mapId;
            X = x;
            Y = y;
            MovementSpeed = mvoementSpeed;
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
        private SpriteRenderer _renderer;
        private SpeedComponent _speedComponent;

        private Queue<Vector2> _movementUpdates = new Queue<Vector2>();

        public OnlinePlayerControllerComponent(OnlinePlayerData metadata)
        {
            Data = metadata;
        }

        public override void OnAddedToEntity()
        {
            Entity.SetPosition(new Vector2(Data.X, Data.Y));

            _mover = Entity.AddComponent<Mover>();
            _renderer = Entity.AddComponent(new PrototypeSpriteRenderer(16f, 16f));
            _renderer.Color = Color.Red;

            _speedComponent = new SpeedComponent(Data.MovementSpeed);

            Logger.Print($"'{Data.CharacterName}' has joined our world!", LogEntryType.Network);
        }

        public void Update()
        {
            if (_movementUpdates.TryDequeue(out Vector2 input))
            {
                // todo: have the realmserver send a speed for each player upon creating the actor.
                var moveDirection = Data.MovementSpeed * Time.DeltaTime * input;
                moveDirection.Round();

                _subPixelMovement.Update(ref moveDirection);
                _mover.ApplyMovement(moveDirection);
            }
        }

        public void EnqueuePositionChange(Vector2 input) => _movementUpdates.Enqueue(input);
    }
}
