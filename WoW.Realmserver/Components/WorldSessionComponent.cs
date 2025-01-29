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
    public class WorldSessionComponent : Component, IUpdatable, ITriggerListener
    {
        public PlayerAccount Account;
        public PlayerCharacter Character;

        private SubpixelVector2 _subPixelMovement;
        private CircleCollider _collider;
        private Mover _mover;

        private Vector2 _moveDirection = Vector2.Zero;
        public float MovementSpeed = 100f;

        public Queue<Vector2> InputUpdates = new Queue<Vector2>();

        public List<Entity> AvailableTargets = new List<Entity>();
        public int TargetIndex = -1;

        public WorldSessionComponent(PlayerAccount user)
            => Account = user;

        public void Update()
        {
            if (InputUpdates.TryDequeue(out var input))
            {
                _moveDirection = MovementSpeed * Time.DeltaTime * input;
                _moveDirection.Round();

                _mover.CalculateMovementExcluding(ref _moveDirection, Entity.Scene.FindComponentsOfType<WorldSessionComponent>().Select(x => x.Entity).ToArray(), out var res);
                _subPixelMovement.Update(ref _moveDirection);
                _mover.ApplyMovement(_moveDirection);

                Program.SendToMapFromPlayer(Entity.Name,
                    new RealmClient_NetPositionInputUpdate()
                    {
                        Id = Entity.Name,
                        ResultX = Entity.Transform.Position.X,
                        ResultY = Entity.Transform.Position.Y,
                        MovementX = input.X,
                        MovementY = input.Y,
                        IsTeleportUpdate = false
                    }, DeliveryMethod.Unreliable);

                // show the player the server's resulting calculation.
                Program.SendTo(Entity.Name, new RealmClient_Debug_ServerPosition() { X = Entity.Transform.Position.X, Y = Entity.Transform.Position.Y });
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
    }
}
