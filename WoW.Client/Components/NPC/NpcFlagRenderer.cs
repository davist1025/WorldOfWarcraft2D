using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Components.NPC
{
    public class NpcFlagRenderer : RenderableComponent, IUpdatable
    {
        public override float Width => 32f;
        public override float Height => 32f;

        private bool _shouldRenderIcon = false;

        public override void Render(Batcher batcher, Camera camera)
        {
            // todo: check for flag type (merchant, gossip, etc)
            if (_shouldRenderIcon)
            {
                var mousePos = Input.MousePosition;
                batcher.Draw(Game1.InterfaceTextures["gear_icon"], (Entity.Scene.Camera.MouseToWorldPoint() - new Microsoft.Xna.Framework.Vector2(-5, 15)));
            }
        }

        public void Update()
        {
            var worldTilePos = Entity.Scene.Camera.MouseToWorldPoint();
            var entityRenderer = Entity.GetComponent<SpriteRenderer>();

            if (entityRenderer == null)
            {
                Debug.Error($"{Entity.Name} has no sprite renderer.");
                Entity.RemoveComponent(this);
                return;
            }

            if (entityRenderer.Bounds.Contains(worldTilePos))
                _shouldRenderIcon = true;
            else
                _shouldRenderIcon = false;
        }
    }
}
