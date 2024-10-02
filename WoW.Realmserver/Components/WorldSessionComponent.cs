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

namespace WoW.Realmserver.Components
{
    public class WorldSessionComponent : Component, IUpdatable
    {
        public PlayerAccount Account;
        public PlayerCharacter Character;

        private SubpixelVector2 _subPixelMovement;
        private CircleCollider _collider;
        private Mover _mover;

        private Vector2 _moveDirection = Vector2.Zero;
        public float MovementSpeed = 100f;

        public Queue<Vector2> InputUpdates = new Queue<Vector2>();

        public WorldSessionComponent(PlayerAccount user)
            => Account = user;

        public void Update()
        {
            if (InputUpdates.TryDequeue(out var input))
            {
                _moveDirection = MovementSpeed * Program.DeltaTime * input;
                _mover.CalculateMovement(ref _moveDirection, out var res);

                // todo: this is debug code for collision; check for collision ONLY on the map the player is on.
                if (res.Collider != null)
                    Console.WriteLine($"{Character.Name} collided with {res.Collider.Bounds.ToString()}!");

                _subPixelMovement.Update(ref _moveDirection);
                _mover.ApplyMovement(_moveDirection);

                // todo: only send to players on the same map as the player.

                Program.SendToExcept(Entity.Name,
                    new RealmClient_NetPositionInputUpdate()
                    {
                        Id = Entity.Name,
                        X = Entity.Transform.Position.X,
                        Y = Entity.Transform.Position.Y,
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
