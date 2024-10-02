using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Scripts
{
    public class TargetedTestScript : ActionEventScript
    {
        /// <summary>
        /// Scripts must contain this static "Id" property so Reflection can find the necessary script for a given Creature.
        /// </summary>
        public static string Id => "targeted_script";

        public override void OnTargeted(Entity fromEntity, Entity parentEntity)
        {
            Console.WriteLine("Hello from the test script!");
            //WorldSessionComponent session = null;
            //GameObjectComponent gObject = null;

            //if (fromEntity.HasComponent<WorldSessionComponent>())
            //    session = fromEntity.GetComponent<WorldSessionComponent>();
            //else if (fromEntity.HasComponent<GameObjectComponent>())
            //    gObject = fromEntity.GetComponent<GameObjectComponent>();

            //Console.WriteLine($"{(session != null ? $"{session.Character.Name} has targeted us!" : $"{gObject.Creature.Name} has targeted us!")}");
        }
    }
}
