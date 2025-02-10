using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;

namespace WoW.Client.Scenes
{
    public class LogonScene : Scene
    {
        public override void Initialize()
        {
            AddRenderer(new RenderLayerRenderer(0, 50));
            AddRenderer(new ScreenSpaceRenderer(1, 100));

            CreateEntity("gui").AddComponent<ImGuiController>();

            //CreateEntity("ui").AddComponent<TestUIRendererComponent>();
        }
    }
}
