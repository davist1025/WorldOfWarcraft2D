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
using WoW.Client.Shared.Realm;
using WoW.Realmserver.DB.Model;
using WoW.Server.Shared.Serializable;

namespace WoW.Realmserver
{
    internal class WorldSession : Component, IUpdatable
    {
        public PlayerAccount Account;
        public PlayerCharacter Character;

        private SubpixelVector2 _subPixelMovement;
        private CircleCollider _collider;
        private Mover _mover;

        private Vector2 _moveDirection = Vector2.Zero;
        public float MovementSpeed = 100f;
        // todo: local position within a zone.

        public Queue<Vector2> InputUpdates = new Queue<Vector2>();

        public WorldSession(PlayerAccount user)
            => Account = user;

        public void Update()
        {
            if (InputUpdates.TryDequeue(out var input))
            {
                // todo: figure some way to do collision/invalid movement.
                // probably best to just imitate Nez' 'Mover' class, and then use Tiled on the server.
                _moveDirection = MovementSpeed * Program.DeltaTime * input;

                _mover.CalculateMovement(ref _moveDirection, out var res);
                _mover.ApplyMovement(_moveDirection);

                Program.SendToExcept(Entity.Name,
                    new RealmClient_NetPositionInputUpdate()
                    {
                        Id = Entity.Name,
                        X = Entity.Position.X,
                        Y = Entity.Position.Y,
                    }, DeliveryMethod.Unreliable);
            }

        }

        public void InitializeGameComponents()
        {
            _collider = Entity.AddComponent<CircleCollider>();
            _collider.SetRadius(16f);
            _mover = Entity.AddComponent<Mover>();
        }
    }
}
