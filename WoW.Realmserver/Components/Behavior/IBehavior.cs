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
        public NpcControllerComponent Controller;

        /// <summary>
        /// Invoked after this behavior is added to an NPCs' <see cref="NpcControllerComponent.Behaviors"/>.
        /// </summary>
        public virtual void OnLoad() { }

        public void SetParent(NpcControllerComponent parent)
            => Controller = parent;

        public virtual void OnTargeted(WorldSessionComponent session) { }
    }
}
