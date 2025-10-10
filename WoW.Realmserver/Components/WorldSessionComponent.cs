using LiteNetLib;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Data;
using WoW.Client.Shared.Realm;
using WoW.Realmserver.DB.Model.Characters;
using WoW.Server.Shared.Serializable;

namespace WoW.Realmserver.Components
{
    public class WorldSessionComponent : Component, IUpdatable, ITriggerListener
    {
        public PlayerAccount Account;
        public PlayerCharacter Character;

        private SubpixelVector2 _subPixelMovement;
        private CircleCollider _collider;
        private Mover _mover;

        private Vector2 _moveDirection = Vector2.Zero;

        private float _tickAccumulator = 0f;
        private long _lastProcessedSequence = 0;
        private Queue<ClientRealm_Movement> _movementStateChanges = new Queue<ClientRealm_Movement>();

        public List<Entity> AvailableTargets = new List<Entity>();
        public int TargetIndex = -1;

        public WorldSessionComponent(PlayerAccount user)
            => Account = user;

        public void Update()
        {
            if (_movementStateChanges.TryDequeue(out var inputStateChange))
            {
                _tickAccumulator += inputStateChange.DeltaTime;
                _lastProcessedSequence = inputStateChange.Sequence;

                var vector = new Vector2(inputStateChange.VelocityX, inputStateChange.VelocityY);

                _moveDirection = Program.Configuration.WorldParameters["global_movement_speed"] * Program.DeltaTime * vector;
                _moveDirection.Round();

                _mover.CalculateMovementExcluding(ref _moveDirection, Entity.Scene.FindComponentsOfType<WorldSessionComponent>().Select(x => x.Entity).ToArray(), out var res);
                _subPixelMovement.Update(ref _moveDirection);
                _mover.ApplyMovement(_moveDirection);

                if (_tickAccumulator >= Program.TickRate)
                {
                    _tickAccumulator = 0f;
                    Program.SendTo(Entity.Name, new RealmClient_MovementStateValidation() { ServerCalculation = new Vector2Serializable(Entity.Transform.Position.X, Entity.Transform.Position.Y), Sequence = _lastProcessedSequence });
                }

                if (vector.X < 0f) Character.Direction = (int)SpriteDirection.West;

                if (vector.X > 0f) Character.Direction = (int)SpriteDirection.East;

                if (vector.Y > 0f) Character.Direction = (int)SpriteDirection.South;

                if (vector.Y < 0f) Character.Direction = (int)SpriteDirection.North;

                Program.SendToMapFromPlayer(Entity.Name,
                    new RealmClient_MovementStateChange()
                    {
                        Id = Entity.Name,
                        ResultX = Entity.Transform.Position.X,
                        ResultY = Entity.Transform.Position.Y,
                        MovementX = vector.X,
                        MovementY = vector.Y,
                        Direction = Character.Direction,
                        IsTeleportUpdate = false
                    }, DeliveryMethod.Unreliable);
            }
        }

        public void InitializeGameComponents()
        {
            _collider = Entity.AddComponent<CircleCollider>();
            _collider.SetRadius(8f);
            _mover = Entity.AddComponent<Mover>();

            Entity.SetPosition(new Vector2(Character.XPosition, Character.YPosition));
        }

        public void OnTriggerEnter(Collider other, Collider local)
            => AvailableTargets.AddIfNotPresent(other.Entity);

        public void OnTriggerExit(Collider other, Collider local)
        {
            AvailableTargets.Remove(other.Entity);

            if (TargetIndex > AvailableTargets.Count)
                TargetIndex = AvailableTargets.Count - 1;
        }

        public void AddMovementStateChange(ClientRealm_Movement stateChange)
            => _movementStateChanges.Enqueue(stateChange);
    }
}
