using Nez;
using Nez.AI.BehaviorTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Behavior
{
    /// <summary>
    /// Contains basic actions the player can take on this NPC.
    /// </summary>
    public class IBehavior
    {
        public Entity Parent;

        /// <summary>
        /// Invoked after this behavior is added to an NPCs' <see cref="BehaviorComponent"/>.
        /// </summary>
        public virtual void OnLoad() { }

        public void SetParent(Entity parent)
            => Parent = parent;

        public virtual void OnTargeted(WorldSessionComponent session) { }
    }
}
