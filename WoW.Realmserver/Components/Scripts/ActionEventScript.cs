using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Scripts
{
    /// <summary>
    /// Contains handlers for common actions other entities will take upon another entity.
    /// </summary>
    public class ActionEventScript
    {
        /// <summary>
        /// Occurs when an entity performs an action-key request on this object.
        /// </summary>
        /// <param name="fromEntity"></param>
        public virtual void OnAction(Entity fromEntity, Entity parentEntity) { }

        /// <summary>
        /// Occurs when this object is targeted.
        /// </summary>
        /// <param name="fromEntity"></param>
        /// <notes>you are stinky and i love you so so much i put this in notes so the stupid computer wont delete it :p</notes>
        public virtual void OnTargeted(Entity fromEntity, Entity parentEntity) { }

    }
}
