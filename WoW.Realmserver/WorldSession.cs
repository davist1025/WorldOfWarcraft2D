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
    internal class WorldSession : ComponentHeadless, IUpdatable
    {
        public PlayerAccount Account { get; init; }
        public PlayerCharacter Character;

        /// <summary>
        /// The global position of this player.
        /// </summary>
        public Vector2 WorldPosition = new Vector2();
        private Vector2 _moveDirection;

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
                WorldPosition += _moveDirection;

                Program.SendToExcept(Entity.Name,
                    new RealmClient_NetPositionInputUpdate()
                    {
                        Id = Entity.Name,
                        X = WorldPosition.X,
                        Y = WorldPosition.Y,
                    }, DeliveryMethod.Unreliable);
            }
        }
    }
}
