using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Behavior
{
    // todo: add update order to updateable behaviors.
    public class IUpdateableBehavior : IBehavior
    {
        public virtual void Update() { }
    }
}
