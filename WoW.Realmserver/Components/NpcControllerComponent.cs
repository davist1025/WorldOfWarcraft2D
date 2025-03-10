using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;
using WoW.Realmserver.Components.Behavior;

namespace WoW.Realmserver.Components
{
    public class NpcControllerComponent : Component, IUpdatable
    {
        public NpcMetadata Metadata { get; init; }
        public List<IBehavior> Behaviors = new List<IBehavior>();
        public SpawnerComponent Spawner;
        public Vector2 SpawnPosition = Vector2.Zero;

        public void Update()
        {
            var updateableBehaviors = Behaviors.FindAll(b => b.GetType().IsAssignableTo(typeof(IUpdateableBehavior))).ToArray();
            foreach (var behavior in updateableBehaviors)
                ((IUpdateableBehavior)behavior).Update();
        }

        public void AddBehavior(IBehavior behavior)
        {
            behavior.SetParent(Entity);
            behavior.OnLoad();

            Behaviors.Add(behavior);
        }

        public void SetHome(SpawnerComponent component)
            => Spawner = component;
    }
}
