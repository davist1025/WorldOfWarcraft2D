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
using WoW.Client.Utils;
using WoW.Framework.Logging;
using WoW.Framework.Shared.Components;

namespace WoW.Client.Components.Player
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
        private SpeedComponent _speed;

        private GameManager _gameManager;

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

            _renderer = Entity.AddComponent(new PrototypeSpriteRenderer(16f, 16f));
            _renderer.Color = Color.Cornsilk;

            _gameManager = Core.GetGlobalManager<GameManager>();

            _speed = (_gameManager.PeerState == GameNetworkState.Offline_World) ?
                Entity.AddComponent(new SpeedComponent(100f)) 
                : Entity.GetComponent<SpeedComponent>();
        }

        public void Update()
        {
            _movementInput = new Vector2(_xAxis.Value, _yAxis.Value);

            if (_movementInput != Vector2.Zero && !ImGui.IsAnyItemActive())
            {
                _gameManager.LocalPlayerMoved?.Invoke(null, _movementInput);

                var moveDirection = _speed.Speed * Time.DeltaTime * _movementInput;
                moveDirection.Round();

                _subPixelMovement.Update(ref moveDirection);
                _mover.ApplyMovement(moveDirection);
            }
        }
    }
}
