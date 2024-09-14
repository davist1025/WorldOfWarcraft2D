using Nez;
using Nez.ECS.Headless;
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

        private Dictionary<string, TiledMapProcessor> _mapProcessors;

        public WorldContentManager()
        {
            _mapProcessors = new Dictionary<string, TiledMapProcessor>();
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
            string mapName = map.Properties["name"];

            // todo: check for an empty name prop.

            // todo: each map can be loaded and placed at 0,0; collision checks occur for each player independently and only within the map their on.
            // need to add support for this later, so players don't collider with every map.
            // random note: the server could hypothetically only load maps that contains at least one player, and unload them when there are none?
            Entity mapEntity = CoreHeadless.Scene.CreateEntity(mapName);
            TiledMapProcessor processor = new TiledMapProcessor(map, "collision_layer");
            mapEntity.AddComponent(processor);

            _mapProcessors.Add(mapName, new TiledMapProcessor(map, "collision_layer"));
        }

        public TiledMapProcessor GetMap(string name)
        {
            if (_mapProcessors.ContainsKey(name))
                return _mapProcessors[name];
            return null;
        }
    }
}
