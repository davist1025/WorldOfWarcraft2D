using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
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
        /// <summary>
        /// The database Metadata for the NPC.
        /// </summary>
        public NpcMetadata Metadata { get; init; }

        /// <summary>
        /// The TiledMap this NPC is on.
        /// </summary>
        public TiledMapProcessor Processor { get; init; }
        public List<IBehavior> Behaviors = new List<IBehavior>();
        public SpawnerComponent Spawner;
        public Vector2 SpawnPosition = Vector2.Zero;
        public Mover Mover;

        public override void OnAddedToEntity()
        {
            Mover = Entity.AddComponent<Mover>();
        }

        public void Update()
        {
            var updateableBehaviors = Behaviors.FindAll(b => b.GetType().IsAssignableTo(typeof(IUpdateableBehavior))).ToArray();
            foreach (var behavior in updateableBehaviors)
                ((IUpdateableBehavior)behavior).Update();
        }

        public void AddBehavior(IBehavior behavior)
        {
            behavior.SetParent(this);
            behavior.OnLoad();

            Behaviors.Add(behavior);
        }

        public void SetHome(SpawnerComponent component)
            => Spawner = component;
    }
}
