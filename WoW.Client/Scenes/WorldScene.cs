using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components.GUI;
using WoW.Client.Components.Player;
using WoW.Client.Utils;

namespace WoW.Client.Scenes
{
    /// <summary>
    /// The primary gameplay scene. Can handle online and offline mode.
    /// </summary>
    internal class WorldScene : Scene
    {
        private GameManager _gameManager;

        public override void Initialize()
        {
            _gameManager = Core.GetGlobalManager<GameManager>();

            var thePlayer = AddEntity(_gameManager.Player);
            thePlayer.AddComponent<MyPlayerControllerComponent>();

            Camera.Entity.AddComponent(new FollowCamera(thePlayer, Camera));
            Camera.Zoom = 0.5f;
            Camera.GetComponent<FollowCamera>().FollowLerp = 0.05f;

            CreateEntity("gui").AddComponent<ImGuiGameManagerComponent>();
        }

        public override void Update()
        {
            base.Update();

            // add new entities to the world as soon as we can.
            if (_gameManager.PopUntrackedActor(out var newActor))
                AddEntity(newActor);
        }
    }
}
