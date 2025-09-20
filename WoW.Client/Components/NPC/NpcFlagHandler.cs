using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Components.NPC
{
    public class NpcFlagHandler : Component, IUpdatable
    {
        private NpcController _npcParent;
        private MouseCursor _renderingCursor = null;

        public override void OnAddedToEntity()
        {
            _npcParent = Entity.GetComponent<NpcController>();
        }

        public void Update()
        {
            var mouseToWorldPos = Entity.Scene.Camera.MouseToWorldPoint();
            var entityRenderer = Entity.GetComponent<SpriteRenderer>();

            if (entityRenderer == null)
            {
                Debug.Error($"{Entity.Name} has no sprite renderer.");
                Entity.RemoveComponent(this);
                return;
            }

            OnHover(entityRenderer.Bounds.Contains(mouseToWorldPos));
            Mouse.SetCursor(_renderingCursor);

            // todo: check for action taken with this npc, using the flag (send packets, open UI, etc)
        }

        private void OnHover(bool isEnter)
        {
            if (isEnter)
            {
                var flags = _npcParent.Metadata.Flags;

                if (flags.HasFlag(Shared.NpcTypeFlags.IsMerchant))
                    _renderingCursor = MouseCursor.FromTexture2D(Game1.InterfaceTextures["merchant_bag_icon"], 0, 0);
            }
            else
                _renderingCursor = MouseCursor.FromTexture2D(Game1.InterfaceTextures["hand1_mouse"], 0, 0);
        }
    }
}
