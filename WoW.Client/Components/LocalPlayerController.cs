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
using WoW.Network.Packets;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using static WoW.Framework.Utils;

namespace WoW.Client.Components
{
    public class LocalPlayerController : Component, IUpdatable
    {
        private VirtualIntegerAxis _xAxis, _yAxis;
        private Vector2 _movementInput;
        private SubpixelVector2 _subPixelMovement;
        private ActorAnimationDirection _direction = ActorAnimationDirection.South;

        private Mover _mover;
        private CircleCollider _circleCollder;
        private SpriteAnimator _animator;

        private long _sequenceCounter = 0;
        private List<ClientRealm_Movement> _unprocessedInput = new List<ClientRealm_Movement>();
        private Queue<RealmClient_MovementStateValidation> _validatedInput = new Queue<RealmClient_MovementStateValidation>();

        public string Name;
        public string TargetWorldId = "";
        private Vector2 _renderPos = Vector2.Zero;

        /** Debug variables **/
        public Vector2 LastServerCalculation = Vector2.Zero;

        public LocalPlayerController(string name, ActorAnimationDirection direction)
        {
            Name = name;
            _direction = direction;
        }

        public override void OnAddedToEntity()
        {
            _xAxis = new VirtualIntegerAxis();
            _xAxis.AddKeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D);
            _xAxis.AddGamePadLeftStickX();
            _yAxis = new VirtualIntegerAxis();
            _yAxis.AddKeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S);
            _yAxis.AddGamePadLeftStickY();
            _subPixelMovement = new SubpixelVector2();

            _movementInput = Vector2.Zero;
            _mover = Entity.AddComponent<Mover>();
            //_circleCollder = Entity.AddComponent(new CircleCollider(8f));
            _animator = Entity.GetComponent<SpriteAnimator>();
            _animator.Speed = 0.5f;
        }

        public void Update()
        {
            if (!ImGui.IsAnyItemActive())
                _movementInput = new Vector2(_xAxis.Value, _yAxis.Value);
            _animator.Update();

            if (!_animator.IsRunning)
            {
                string startingAnimation = "";
                switch (_direction)
                {
                    case ActorAnimationDirection.North:
                    case ActorAnimationDirection.East:
                    case ActorAnimationDirection.South:
                    case ActorAnimationDirection.West:
                        startingAnimation = "idle";
                        break;
                }
                _animator.Play(startingAnimation); // avoids a null-reference exception.
            }

            if (_movementInput != Vector2.Zero)
            {
                if (_movementInput.X < 0f)
                {
                    _direction = ActorAnimationDirection.West;
                    _animator.FlipX = true;
                }

                if (_movementInput.X > 0f)
                {
                    _direction = ActorAnimationDirection.East;
                    _animator.FlipX = false;
                }

                if (_movementInput.Y > 0f) _direction = ActorAnimationDirection.South;

                if (_movementInput.Y < 0f) _direction = ActorAnimationDirection.North;

                switch (_direction)
                {
                    case ActorAnimationDirection.North:
                    case ActorAnimationDirection.East:
                    case ActorAnimationDirection.South:
                    case ActorAnimationDirection.West:
                        if (!_animator.CurrentAnimationName.Equals("walk"))
                            _animator.Play("walk");
                        break;
                }

                ApplyInput(_movementInput);

                var movementUpdatePacket = new ClientRealm_Movement()
                {
                    VelocityX = _movementInput.X,
                    VelocityY = _movementInput.Y,
                    Sequence = _sequenceCounter++,
                    DeltaTime = Time.DeltaTime
                };
                _unprocessedInput.Add(movementUpdatePacket);
                Global.Network.SendToServer(movementUpdatePacket);
            }

            if (_movementInput == Vector2.Zero)
            {
                switch (_direction)
                {
                    case ActorAnimationDirection.North:
                    case ActorAnimationDirection.East:
                    case ActorAnimationDirection.South:
                    case ActorAnimationDirection.West:
                        if (!_animator.CurrentAnimationName.Equals("idle"))
                            _animator.Play("idle");
                        break;
                }

                if (_sequenceCounter > 0)
                    _sequenceCounter = 0;
            }

            if (_validatedInput.TryDequeue(out var result))
            {
                Entity.Transform.Position = result.ServerCalculation.ToVector2XNA();
                _unprocessedInput.RemoveAll(x => x.Sequence <= result.Sequence);

                foreach (var unproccessedInput in _unprocessedInput)
                    ApplyInput(new Vector2(unproccessedInput.VelocityX, unproccessedInput.VelocityY));
            }
        }

        public void ProcessInputValidation(RealmClient_MovementStateValidation validation)
            => _validatedInput.Enqueue(validation);

        private void ApplyInput(Vector2 input)
        {
            var moveDirection = Global.Speed * Time.DeltaTime * input;
            moveDirection.Round();

            //_mover.CalculateMovement(ref moveDirection, out var _);
            //_mover.CalculateMovementExcluding(ref moveDirection, Entity.Scene.FindComponentsOfType<NetPlayerController>().Select(x => x.Entity).ToArray(), out var res);
            _subPixelMovement.Update(ref moveDirection);
            _mover.ApplyMovement(moveDirection);
        }

        public override void DebugRender(Batcher batcher)
        {
            batcher.DrawHollowRect(_animator.LastNetworkPosition, 32f, 32f, Color.Yellow);
            // todo: need to set an "Origin" value server-side so this is automatically calculated and the position matches what the client would expect.
            //batcher.DrawHollowRect(LastServerPosition - new Vector2(16f / 2f), 16f, 16f, Color.Red);

            //if (!string.IsNullOrEmpty(TargetWorldId))
            //{
            //    var entity = Entity.Scene.FindEntity(TargetWorldId);

            //    if (entity != null)
            //    {
            //        batcher.DrawLine(Entity.Position, entity.Position, Color.Yellow);
            //    }
            //}
        }
    }
}
