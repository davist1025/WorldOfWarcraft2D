using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;

namespace WoW.Client.Scenes
{
    /// <summary>
    /// The primary gameplay scene. Can handle online and offline mode.
    /// </summary>
    internal class WorldScene : Scene
    {
        public override void Initialize()
        {
            var thePlayer = AddEntity(Global._Player);
            thePlayer.AddComponent<MyPlayerControllerComponent>();

            Camera.Entity.AddComponent(new FollowCamera(thePlayer, Camera));
            Camera.Zoom = 0.5f;
            Camera.GetComponent<FollowCamera>().FollowLerp = 0.05f;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
