using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Client;

namespace WoW.Client.Components
{

    public class LocalPlayerController : Component, IUpdatable
    {
        private VirtualIntegerAxis _xAxis, _yAxis;
        private Vector2 _movementInput;
        private SubpixelVector2 _subPixelMovement;
        private SpriteDirection _direction;

        private Mover _mover;
        private CircleCollider _circleCollder;
        private SpriteAnimator _animator;

        private int _tickCount = 0;
        //private List<InputChangeTick> _inputRecord;

        public string Name;
        public Vector2 LastServerPosition = Vector2.Zero;
        public string TargetWorldId = "";

        public LocalPlayerController(string name)
            => Name = name;

        public override void OnAddedToEntity()
        {
            _xAxis = new VirtualIntegerAxis();
            _xAxis.AddKeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D);
            _yAxis = new VirtualIntegerAxis();
            _yAxis.AddKeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S);
            _subPixelMovement = new SubpixelVector2();

            _movementInput = Vector2.Zero;
            _mover = Entity.AddComponent<Mover>();
            _circleCollder = Entity.AddComponent(new CircleCollider(8f));
            _animator = Entity.GetComponent<SpriteAnimator>();
            // todo: should collider size be set by the server and transmitted?
        }

        public void Update()
        {
            _movementInput = new Vector2(_xAxis.Value, _yAxis.Value);

            if (_movementInput != Vector2.Zero)
            {
                ++_tickCount; // todo: unused client tick.
                // todo: are more generalized state updates needed (i.e sending all inputs that matter; movement, attack, etc)?
                // for instance: we could send new input changes such as "started moving left" once, and then once more when we release the button.
                //_inputRecord.Add(new InputChangeTick(_tickCount, _movementInput));
                Game1.Send(new ClientRealm_Movement() { X = _movementInput.X, Y = _movementInput.Y, Tick = _tickCount }, LiteNetLib.DeliveryMethod.Unreliable);
                
                var moveDirection = Game1.MovementSpeed * Time.DeltaTime * _movementInput;
                moveDirection.Round();

                if (_movementInput.X < 0f) _direction = SpriteDirection.West;

                if (_movementInput.X > 0f) _direction = SpriteDirection.East;

                if (_movementInput.Y > 0f) _direction = SpriteDirection.South;

                if (_movementInput.Y < 0f) _direction = SpriteDirection.North;

                switch (_direction)
                {
                    case SpriteDirection.North:
                        _animator.Play("idle_north");
                        break;
                    case SpriteDirection.East:
                        _animator.Play("idle_east");
                        break;
                    case SpriteDirection.South:
                        _animator.Play("idle_south");
                        break;
                    case SpriteDirection.West:
                        _animator.Play("idle_west");
                        break;
                }

                _mover.CalculateMovementExcluding(ref moveDirection, Entity.Scene.FindComponentsOfType<NetPlayerController>().Select(x => x.Entity).ToArray(), out var res);
                _subPixelMovement.Update(ref moveDirection);
                _mover.ApplyMovement(moveDirection);
            }

            if (_movementInput == Vector2.Zero && _tickCount > 0)
                _tickCount = 0;
        }

        public override void DebugRender(Batcher batcher)
        {
            // todo: need to set an "Origin" value server-side so this is automatically calculated and the position matches what the client would expect.
            batcher.DrawHollowRect(LastServerPosition - new Vector2(16f / 2f), 16f, 16f, Color.Red);

            if (!string.IsNullOrEmpty(TargetWorldId))
            {
                var entity = Entity.Scene.FindEntity(TargetWorldId);

                if (entity != null)
                {
                    batcher.DrawLine(Entity.Position, entity.Position, Color.Yellow);
                }
            }
        }
    }
}
