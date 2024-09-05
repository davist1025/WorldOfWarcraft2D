using Nez.Tiled;
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
        private const string _rootDirectory = "Content\\Data";

        public WorldContentManager()
        {
            LoadTiled();
            // todo: verify integrity.
            // the realmserver should come equipped with the files it will need upon startup. Most of these, such as Tiled maps, cannot be generated.
        }

        // todo: adapt TiledMapLoader to load files in a headless state.
        private void LoadTiled()
        {
            var tiledMaps = Directory.GetFiles($"{_rootDirectory}\\Tiled");
            for (int i = 0; i < tiledMaps.Length; i++)
                LoadTiledMap(tiledMaps[i]);
        }

        private TmxMap LoadTiledMap(string name)
        {
            Console.WriteLine($"Loading tiled map {name}");
            return null;
        }
    }
}
