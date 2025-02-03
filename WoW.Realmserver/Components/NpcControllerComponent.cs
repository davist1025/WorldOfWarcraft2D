using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;

namespace WoW.Realmserver.Components
{
    public class NpcControllerComponent : Component, IUpdatable
    {
        public NpcMetadata Metadata { get; init; }

        public override void OnAddedToEntity()
        {
            // todo: add Mover to NPC.
            //var trigger = Entity.AddComponent(new CircleCollider(64f));
            //trigger.IsTrigger = true;
        }

        public void Update()
        {
        }
    }
}
