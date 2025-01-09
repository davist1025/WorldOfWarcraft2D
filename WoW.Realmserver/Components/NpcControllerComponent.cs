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
        public RemoteNPC Data { get; init; }

        public override void OnAddedToEntity()
        {
            // todo: add Mover to NPC.
        }

        public void Update()
        {
        }
    }
}
