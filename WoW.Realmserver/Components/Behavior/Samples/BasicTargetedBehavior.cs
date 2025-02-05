using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Realm;

namespace WoW.Realmserver.Components.Behavior.Samples
{
    /// <summary>
    /// Once targeted, follow the one targeting us until we reach their position.
    /// 
    /// This is an example behavior that shows how to build a simple one and run a continuous task.
    /// </summary>
    [Behavior("basic_targeted")]
    public class BasicTargetedBehavior : IUpdateableBehavior
    {
        private WorldSessionComponent _followingPlayer;
        private Mover _mover;
        private bool _shouldFollow = false;

        public override void OnLoad()
        {
            _mover = Parent.AddComponent<Mover>();
        }

        public override void Update()
        {
            if (_followingPlayer != null)
            {
                var distance = _followingPlayer.Entity.Position - Parent.Position;
                distance.Normalize();

                if (_shouldFollow)
                {
                    // - 15f for a slower NPC movement.
                    var movement = distance * Time.DeltaTime * (Program.Configuration.WorldParameters["global_movement_speed"] - 15f);
                    Console.WriteLine(movement);

                    _mover.CalculateMovement(ref movement, out CollisionResult collisionResult);
                    _mover.ApplyMovement(movement);

                    // hack: temporary call to update the NPC position to clients while testing the movement code on the server
                    Program.SendToAll(new RealmClient_NetPositionInputUpdate()
                    {
                        Id = Parent.Name,
                        ResultX = Parent.Position.X,
                        ResultY = Parent.Position.Y,
                        MovementX = distance.X,
                        MovementY = distance.Y,
                        IsTeleportUpdate = false
                    });

                }

                _shouldFollow = (Vector2.Distance(Parent.Position, _followingPlayer.Entity.Position) < 1f) ? false : true;
            }
        }

        public override void OnTargeted(WorldSessionComponent session)
        {
            Console.WriteLine($"{session.Character.Name} targeted us, moving towards them...");

            _followingPlayer = session;
        }
    }
}
