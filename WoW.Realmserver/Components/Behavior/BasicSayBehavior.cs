using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Behavior
{
    /// <summary>
    /// Intended to show NPC dialogue in the game chat.
    /// </summary>
    [Behavior("testsayscript")]
    public class BasicSayBehavior : IBehavior
    {
        public override void OnLoad()
        {
            base.OnLoad();

            Console.WriteLine($"Successfully loaded: {GetType().GetCustomAttribute<BehaviorAttribute>().Id}");
        }

        public override void OnTargeted(WorldSessionComponent session)
        {
            Console.WriteLine($"{session.Character.Name} is targeting us, {Parent.Name}");

            // todo: replace with a chat message.
        }
    }
}
