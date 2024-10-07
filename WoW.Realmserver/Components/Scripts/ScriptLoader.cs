using Nez;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Scripts
{
    /// <summary>
    /// Manages all script files.
    /// </summary>
    public class ScriptLoader
    {
        private static List<ActionEventScript> _scriptsFromAssembly = new List<ActionEventScript>();

        /// <summary>
        /// Loads a script from the Data directory.
        /// </summary>
        /// <param name="fileName"></param>
        public static void Load(string fileName)
            => throw new NotImplementedException("Loading a script from file has not been implemented.");

        /// <summary>
        /// Loads a script file from this Assembly, instantiates and returns it.
        /// Typically used to attach to a GameObjectComponent.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ActionEventScript GetScriptFromAssembly(string id)
        {
            // hack: script loading from assm is probably _really_ slow but it's fine since it's only used once when loading a creature for the first time.

            ActionEventScript scriptToReturn = null;
            Type[] childTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsAssignableTo(typeof(ActionEventScript))).ToArray();

            for (int i = 0; i < childTypes.Length; i++)
            {
                var type = childTypes[i];

                PropertyInfo[] idProperty = type
                    .GetProperties(BindingFlags.Public | BindingFlags.Static);

                for (int j = 0; j < idProperty.Length; j++)
                {
                    PropertyInfo pi = idProperty[j];
                    if (pi.Name == "Id")
                    {
                        string typeIdPropValue = (string)pi.GetValue(null);
                        if (string.Equals(typeIdPropValue, id, StringComparison.OrdinalIgnoreCase))
                            scriptToReturn = (ActionEventScript)Activator.CreateInstance(type);
                    }
                }
            }
            return scriptToReturn;
        }
    }
}
