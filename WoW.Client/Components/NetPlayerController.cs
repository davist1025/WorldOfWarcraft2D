using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Realm;

namespace WoW.Client.Components
{
    public class NetPlayerController : Component, IUpdatable
    {
        public string Name;
        public int RaceId;
        public int HairId;

        private PrototypeSpriteRenderer _renderer;
        private Vector2 _currentMoveDirection = Vector2.Zero;
        private SubpixelVector2 _subPixelMovement;
        private Mover _mover;
        private CircleCollider _circleCollider;

        public Queue<Vector2> MovementDirectionQueue = new Queue<Vector2>();

        public NetPlayerController(RealmClient_CreateNetPlayer networkPlayer)
        {
            Name = networkPlayer.Name;
            RaceId = networkPlayer.RaceId;
            HairId = networkPlayer.HairId;
        }

        public override void OnAddedToEntity()
        {
            _subPixelMovement = new SubpixelVector2();

            _mover = Entity.AddComponent<Mover>();
            _circleCollider = Entity.AddComponent<CircleCollider>();
            _circleCollider.SetRadius(8f);
        }

        public void Update()
        {
            if (MovementDirectionQueue.TryDequeue(out Vector2 serverOut))
            {
                Vector2 movement = new Vector2(serverOut.X, serverOut.Y);
                var direction = Game1.MovementSpeed * Time.DeltaTime * movement;

                _mover.CalculateMovement(ref direction, out var _);
                _subPixelMovement.Update(ref direction);
                _mover.ApplyMovement(direction);
            }
        }
    }
}
