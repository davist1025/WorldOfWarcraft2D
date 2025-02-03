using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Behavior
{
    public class BehaviorComponent : Component, IUpdatable
    {
        public List<IBehavior> Behaviors = new List<IBehavior>();

        public void Update()
        {
            // todo: run behavior tree?
        }

        public void AddBehavior(IBehavior behavior)
        {
            behavior.SetParent(Entity);
            behavior.OnLoad();

            Behaviors.Add(behavior);
        }
    }
}
