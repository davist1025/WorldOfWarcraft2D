using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Serializable;

namespace WoW.Client.Components
{
    internal class LocalPlayerController : Component, IUpdatable
    {
        private SerializableCharacter _character;
        private PrototypeSpriteRenderer _renderer;

        // todo: create a renderer based on the character race.

        private VirtualIntegerAxis _xAxis, _yAxis;
        private Vector2 _movementInput;
        private SubpixelVector2 _subPixelMovement;
        private Mover _mover;
        private CircleCollider _circleCollder;

        private int _tickCount = 0;

        public LocalPlayerController(SerializableCharacter character)
            => _character = character;

        public override void OnAddedToEntity()
        {
            _renderer = Entity.AddComponent(new PrototypeSpriteRenderer(16f, 16f));

            _xAxis = new VirtualIntegerAxis();
            _xAxis.AddKeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D);
            _yAxis = new VirtualIntegerAxis();
            _yAxis.AddKeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S);
            _subPixelMovement = new SubpixelVector2();

            _movementInput = Vector2.Zero;
            _mover = Entity.AddComponent<Mover>();
            _circleCollder = Entity.AddComponent<CircleCollider>();
            _circleCollder.SetRadius(16f);
        }

        public void Update()
        {
            _movementInput = new Vector2(_xAxis.Value, _yAxis.Value);

            if (_movementInput != Vector2.Zero)
            {
                ++_tickCount; // todo: unused client tick.
                // todo: are more generalized state updates needed (i.e sending all inputs that matter; movement, attack, etc)?
                // for instance: we could send new input changes such as "started moving left" once, and then once more when we release the button.
                Game1.Send(new ClientRealm_Movement() { X = _movementInput.X, Y = _movementInput.Y }, LiteNetLib.DeliveryMethod.Unreliable);

                // todo: '100f' should come from the server; this is our movement speed.
                var moveDirection = 100f * Time.DeltaTime * _movementInput;

                _mover.CalculateMovement(ref moveDirection, out var _);
                _subPixelMovement.Update(ref moveDirection);
                _mover.ApplyMovement(moveDirection);
            }
        }

        /// <summary>
        /// Returns the character name for the player.
        /// todo: temporary function to quickly grab that character name for our player.
        /// </summary>
        /// <returns></returns>
        public string GetName()
            => _character.Name;
    }
}
