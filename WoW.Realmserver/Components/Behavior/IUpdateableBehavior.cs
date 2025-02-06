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
        public int UpdateOrder = 0;
        // UpdateOrders can be set automatically through iteration.
        // These can update sequentially incase some behaviors need to run before others.
        // UpdateOrder should be set upon object creation. Sample or default scripts will set this automatically.

        public virtual void Update() { }
    }
}
