using Nez;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Components
{
    /// <summary>
    /// An object used for testing ninepatch images.
    /// 
    /// Will come back to this later when building UI.
    /// </summary>
    public class TestUIRendererComponent : RenderableComponent
    {
        public override float Width => 64f;

        public override float Height => 64f;

        private NinePatchDrawable _ninePatch;

        public override void OnAddedToEntity()
        {
            _ninePatch = new NinePatchDrawable(Entity.Scene.Content.LoadTexture("Content/Data/window_ninepatch_1.png"), 4, 4, 4, 4);
            RenderLayer = 100;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            _ninePatch.Draw(batcher, 50f, 50f, 400f, 150f, Microsoft.Xna.Framework.Color.White);
        }
    }
}
