using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;

namespace WoW.Client.Components
{
    /// <summary>
    /// The local player component.
    /// </summary>
    public class MyPlayerControllerComponent : Component, IUpdatable
    {
        private VirtualIntegerAxis _xAxis, _yAxis;
        private Vector2 _movementInput;
        private SubpixelVector2 _subPixelMovement;

        private Mover _mover;

        private PrototypeSpriteRenderer _renderer;

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

            // todo: [player controller] temporarily add a prototype renderer.
            _renderer = Entity.AddComponent(new PrototypeSpriteRenderer(16f, 16f));
            _renderer.Color = Color.MonoGameOrange;

            Logger.Print($"Entering world w/ character: ({Global.GetSelectedCharacter().Name})", Framework.Utils.LogEntryType.Debug);
        }

        public void Update()
        {
            _movementInput = new Vector2(_xAxis.Value, _yAxis.Value);

            var moveDirection = 100f * Time.DeltaTime * _movementInput;
            moveDirection.Round();

            _subPixelMovement.Update(ref moveDirection);
            _mover.ApplyMovement(moveDirection);

            // todo: make an emitter, similar to Core.Emitter, for game-spricifc events.
        }
    }
}
