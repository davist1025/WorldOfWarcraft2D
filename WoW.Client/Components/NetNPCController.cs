using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;

namespace WoW.Client.Components
{
    public class NetNPCController : Component, IUpdatable
    {
        public RemoteNPC Remote;

        public NetNPCController(RemoteNPC remote)
            => Remote = remote;

        public override void OnAddedToEntity()
        {
            var rendererer = Entity.AddComponent(new PrototypeSpriteRenderer(16f, 16f));
            rendererer.SetColor(Color.LightBlue);

            // utilized only to test trigger sights.
            //var trigger = Entity.AddComponent(new CircleCollider(64f));
            //trigger.IsTrigger = true;

            Entity.SetPosition(new Vector2(Remote.X, Remote.Y));
        }

        public void Update()
        {
        }
    }
}
