using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Content
{
    /// <summary>
    /// Handles file management for the realmserver.
    /// </summary>
    internal class WorldContentManager
    {
        private const string _rootDirectory = "Data";

        public WorldContentManager()
        {
            // todo: verify integrity.
            // the realmserver should come equipped with the files it will need upon startup. Most of these, such as Tiled maps, cannot be generated.
        }
    }
}
