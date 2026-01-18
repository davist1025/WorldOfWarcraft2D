using LiteNetLib;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models.Auth;
using WoW.Database.Models.Realm.Character;
using WoW.Network.Packets.Realm;
using WoW.Realmserver.Data;
using static WoW.Framework.Utils;

namespace WoW.Realmserver.Components
{
    public class WorldSessionComponent : Component, IUpdatable
    {
        public Account Account;
        public PlayerCharacter Character;

        private SubpixelVector2 _subPixelMovement;
        private CircleCollider _collider;
        private Mover _mover;

        private bool _isColliding = false;
        private Vector2 _moveDirection = Vector2.Zero;

        private float _tickAccumulator = 0f;
        private long _lastProcessedSequence = 0;
        private Queue<ClientMovementUpdate> _movementUpdates = new Queue<ClientMovementUpdate>();

        public List<Entity> AvailableTargets = new List<Entity>();
        public int TargetIndex = -1;

        public WorldSessionComponent(Account user)
            => Account = user;

        public void Update()
        {
            if (_movementUpdates.TryDequeue(out var inputStateChange))
            {
                _tickAccumulator += inputStateChange.DeltaTime;
                _lastProcessedSequence = inputStateChange.Sequence;

                var vector = new Vector2(inputStateChange.X, inputStateChange.Y);

                _moveDirection = Program.Configuration.WorldParameters["global_movement_speed"] * Program.DeltaTime * vector;
                _moveDirection.Round();

                //_mover.CalculateMovement(ref _moveDirection, out var res);
                _subPixelMovement.Update(ref _moveDirection);
                _mover.ApplyMovement(_moveDirection);

                //_isColliding = (res.Collider != null) ? true : false;

                if (_tickAccumulator >= Program.TickRate)
                {
                    _tickAccumulator = 0f;

                    Program.SendTo(Entity.Name, new RealmClient_MovementStateValidation()
                    {
                        ServerCalculation = new Vector2Serializable(Entity.Transform.Position.X, Entity.Transform.Position.Y),
                        Sequence = _lastProcessedSequence
                    });
                }

                if (vector.X < 0f) Character.Direction = (int)ActorAnimationDirection.West;

                if (vector.X > 0f) Character.Direction = (int)ActorAnimationDirection.East;

                if (vector.Y > 0f) Character.Direction = (int)ActorAnimationDirection.South;

                if (vector.Y < 0f) Character.Direction = (int)ActorAnimationDirection.North;

                Program.SendToExcept(Entity.Name,
                    new RealmClient_MovementStateChange()
                    {
                        Id = Entity.Name,
                        ResultX = Entity.Transform.Position.X,
                        ResultY = Entity.Transform.Position.Y,
                        IsColliding = false, // hack: temporary
                        //ColliderNormal = (_isColliding) ? new Vector2Serializable(res.Normal.X, res.Normal.Y) : new Vector2Serializable(0f, 0f),
                        MovementX = vector.X,
                        MovementY = vector.Y,
                        Direction = Character.Direction,
                        IsTeleportUpdate = false
                    }, DeliveryMethod.Unreliable);
            }
        }

        public void InitializeGameComponents()
        {
            //_collider = Entity.AddComponent<CircleCollider>();
            //Flags.SetFlagExclusive(ref _collider.CollidesWithLayers, 10);
            //Flags.SetFlagExclusive(ref _collider.PhysicsLayer, 1);
            //_collider.SetRadius(8f);
            _mover = Entity.AddComponent<Mover>();

            Entity.SetPosition(new Vector2(Character.XPosition, Character.YPosition));
        }

        public void QueueMovementUpdate(ClientMovementUpdate moveUpdate)
            => _movementUpdates.Enqueue(moveUpdate);
    }
}
