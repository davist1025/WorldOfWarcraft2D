using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Realmserver.Components;

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
            // should a tool be created to package/unpackage them from the client?
        }

        public void LoadTiled()
        {
            Console.WriteLine("Loading Tiled maps...");

            var tiledMaps = Directory.GetFiles($"{_rootDirectory}\\Tiled");

            for (int i = 0; i < tiledMaps.Length; i++)
                LoadTiledMap(tiledMaps[i], i);
        }

        private void LoadTiledMap(string name, int physicsLayerIndex)
        {
            TmxMap map = new TmxMap().LoadTmxMapHeadless(name);
            string mapName = map.Properties["id"];

            // todo: check for an empty name prop.
            // need to add support for this later, so players don't collider with every map.
            // random note: the server could hypothetically only load maps that contains at least one player, and unload them when there are none?
            Entity mapEntity = CoreHeadless.Scene.CreateEntity(mapName);
            TiledMapProcessor processor = new TiledMapProcessor(map, "collision_layer");

            processor.PhysicsLayer = (physicsLayerIndex == 0) ? 1 << 0 : 1 << physicsLayerIndex;
            mapEntity.AddComponent(processor);

            Console.WriteLine($"Creating a Tiled processor for: '{mapName}'....");

            _mapProcessors.Add(mapName, new TiledMapProcessor(map, "collision_layer"));

            if (map.ObjectGroups.Count > 0)
            {
                // process mob/player spawners first
                var spawners = map.GetObjectGroup("spawner");

                if (spawners != null)
                {
                    Console.WriteLine($"Adding {spawners.Objects.Count} spawner(s) to {mapName}...");
                    for (int i = 0; i < spawners.Objects.Count; i++)
                    {
                        var spawnObject = spawners.Objects[i];
                        var position = new Vector2(spawnObject.X, spawnObject.Y);
                        var isPlayer = Convert.ToBoolean(spawnObject.Properties["is_player"]);
                        var npcId = Convert.ToInt32(spawnObject.Properties["npc_id"]);
                        var maxCount = Convert.ToInt32(spawnObject.Properties["max_count"]);

                        float timer = 0.0f;

                        if (!isPlayer)
                        {
                            timer = Convert.ToSingle(spawnObject.Properties["timer"]);
                        }

                        var spawnerComp = new SpawnerComponent(processor, npcId, maxCount, timer, isPlayer, position, new Vector2(spawnObject.Width, spawnObject.Height));
                        mapEntity.AddComponent(spawnerComp);
                    }
                }
            }
        }

        public TiledMapProcessor GetMap(string name)
        {
            if (_mapProcessors.ContainsKey(name))
                return _mapProcessors[name];
            return null;
        }
    }
}
