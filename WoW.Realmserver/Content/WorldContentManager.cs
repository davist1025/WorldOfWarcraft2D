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

        private List<TmxMap> _maps;

        public WorldContentManager()
        {
            _maps = new List<TmxMap>();
            // todo: verify integrity.
            // the realmserver should come equipped with the files it will need upon startup. Most of these, such as Tiled maps, cannot be generated.
        }

        public void LoadTiled()
        {
            Console.WriteLine("Loading Tiled maps...");

            var tiledMaps = Directory.GetFiles($"{_rootDirectory}\\Tiled");
            for (int i = 0; i < tiledMaps.Length; i++)
                LoadTiledMap(tiledMaps[i]);
        }

        private void LoadTiledMap(string name)
        {
            TmxMap map = new TmxMap().LoadTmxMapHeadless(name);
            _maps.Add(map);
        }

        public TmxMap GetMap(string name)
            => _maps.Find(m => m.Properties["name"].Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
