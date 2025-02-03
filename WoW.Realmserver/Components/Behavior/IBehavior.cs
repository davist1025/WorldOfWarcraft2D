using Nez;
using Nez.AI.BehaviorTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Behavior
{
    public class IBehavior
    {
        protected Entity Parent;
        protected BehaviorTreeBuilder<IBehavior> Builder;

        /// <summary>
        /// Invoked after this behavior is added to an NPCs' <see cref="BehaviorComponent"/>.
        /// </summary>
        public virtual void OnLoad()
            => BuildRoutine();

        /// <summary>
        /// Used to build this Behavior's BehaviorTree routine.
        /// Not every Behavior will need to implement this.
        /// </summary>
        public virtual void BuildRoutine()
        {
            Builder = new BehaviorTreeBuilder<IBehavior>(this);
        }

        public void SetParent(Entity parent)
            => Parent = parent;

        /// <summary>
        /// Invoked when this NPC is targeted by a player.
        /// </summary>
        public virtual void OnTargeted(WorldSessionComponent session) { }
    }
}
