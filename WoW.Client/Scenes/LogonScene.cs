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
            CreateEntity("gui").AddComponent<ImGuiController>();
        }
    }
}
