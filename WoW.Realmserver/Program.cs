using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using Nez.Systems;
using Nez.Tiled;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using WoW.Database;
using WoW.Database.Models;
using WoW.Database.Models.Auth;
using WoW.Database.Models.Realm.Character;
using WoW.Database.Models.Realm.Items;
using WoW.Framework.Logging;
using WoW.Network;
using WoW.Network.Objects;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using WoW.Realmserver.Components;
using WoW.Realmserver.Content;
using WoW.Realmserver.Data;
using WoW.Realmserver.Network;
using static WoW.Framework.Utils;

namespace WoW.Realmserver
{
    internal class Program : CoreHeadless
    {
        public static WorldContentManager Content;

        public const float TickRate = 0.1f;

        public static Queue<PendingPlayer> PendingPlayers = new Queue<PendingPlayer>();
        public static List<Entity> ItemReferences = new List<Entity>();

        public Program()
        {
            Console.Title = "Realmserver";
            TiledMapLoader.IsHeadless = true;
            Scene.IsHeadless = true;
            IsFixedTimeStep = true;

            Scene = new WorldScene();
            Content = new WorldContentManager();

            // load content.
            Content.LoadTiled();

            EFCoreContext.AppSettings = ConfigurationManager.AppSettings;

            using (var ctx = new RealmContext())
            {
                Entity mapEntity = null;

                Logger.Print("Verifying racial spawn locations...", LogEntryType.Process);
                if (!ctx.RaceSpawns.Any(spawn => spawn.RaceId == (int)ActorRaceType.Human))
                {
                    mapEntity = Scene.FindEntity("elwynn_forest");
                    var spawnerComponent = mapEntity.GetComponents<SpawnerComponent>().Where(spawner => spawner.IsPlayerSpawner).Single();

                    ctx.RaceSpawns.Add(new CharacterRaceSpawn()
                    {
                        RaceId = (int)ActorRaceType.Human,
                        MapId = "elwynn_forest",
                        X = spawnerComponent.Position.X,
                        Y = spawnerComponent.Position.Y,
                    });
                }

                if (!ctx.RaceSpawns.Any(spawn => spawn.RaceId == (int)ActorRaceType.Orc))
                {
                    ctx.RaceSpawns.Add(new CharacterRaceSpawn()
                    {
                        RaceId = (int)ActorRaceType.Orc,
                        MapId = "valley_of_trials",
                        X = 50f,
                        Y = 50f,
                    });
                }

                Logger.Print("Loading all items...", LogEntryType.Process);

                var items = ctx.Items.ToList();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var itemEntityRef = Scene.CreateEntity($"{item.Id}_{item.Name.ToLower()}");
                    var itemComponent = itemEntityRef.AddComponent(new ItemComponent(item));

                    ItemReferences.Add(itemEntityRef);
                }
                Logger.Print($"Loaded {ItemReferences.Count} items.", LogEntryType.Debug);

                ctx.SaveChanges();
            }

            Global.Network = new NetworkManager(new NetworkEventListener());
            Global.Network.StartServer(ConfigurationManager.AppSettings["hostname"].Split(":"));

            while (true)
                Tick();
        }

        public override void Update(float deltaTime)
        {
            Global.DeltaTime = deltaTime;
            Global.Network.Update();
            Scene.Update();
        }

        static void Main(string[] args)
            => new Program();
    }
}
