using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Behavior
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BehaviorAttribute : Attribute
    {
        public string Id;

        public BehaviorAttribute(string id)
            => Id = id;
    }
}
