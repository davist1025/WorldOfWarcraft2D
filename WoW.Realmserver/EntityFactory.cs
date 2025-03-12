using Microsoft.Xna.Framework;
using MySqlX.XDevAPI;
using Nez;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Data;
using WoW.Client.Shared.Realm;
using WoW.Realmserver.Components;
using WoW.Realmserver.Components.Behavior;
using WoW.Realmserver.DB;
using WoW.Realmserver.DB.Model;

namespace WoW.Realmserver
{
    /// <summary>
    /// Creates new instances of varying types of entities.
    /// </summary>
    public static class EntityFactory
    {
        /// <summary>
        /// Creates an NPC given an ID, and instantiates all metadata, including behaviors. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mapId"></param>
        /// <param name="spawnPosition"></param>
        public static Entity CreateNPC(int id, string mapId, Vector2 spawnPosition, bool isCommand = false)
        {
            using (var realmContext = new RealmContext())
            {
                // Gather metadata (base data and behavior data)
                NonPlayerCharacter npcMetadata = realmContext.NPCs.First(npc => npc.Id == id);

                if (npcMetadata == null)
                {
                    Console.WriteLine($"Unable to find existing NPC metadata with the id: {id}!");

                    return null;
                }

                NonPlayerCharacterBehavior[] thisNpcBehaviors = realmContext.NpcBehaviors.Where(behavior => behavior.NpcId == npcMetadata.Id).ToArray();

                // Create the entity.
                Entity newNpcEntity = Program.Scene.CreateEntity(Guid.NewGuid().ToString(), spawnPosition);
                newNpcEntity.Tag = (int)EntityType.NPC;

                NpcMetadata serializedNpc = new NpcMetadata()
                {
                    WorldId = newNpcEntity.Name,
                    Name = npcMetadata.Name,
                    ModelId = npcMetadata.ModelId,
                    Flags = (NpcTypeFlags)npcMetadata.FlagType,
                    Level = npcMetadata.Level,
                    MapId = mapId,
                    X = spawnPosition.X,
                    Y = spawnPosition.Y
                };

                TiledMapProcessor tiledProcessorForMapId = Program
                    .Scene
                    .FindComponentsOfType<TiledMapProcessor>()
                    .Single(processor => processor.Map.Properties["id"].ToLower()
                    .Equals(mapId.ToLower()));

                if (tiledProcessorForMapId != null)
                {
                    var controllerForNpc = newNpcEntity.AddComponent(new NpcControllerComponent
                    {
                        Metadata = serializedNpc,
                    });

                    if (isCommand)
                        controllerForNpc.SpawnPosition = spawnPosition;

                    var behaviorsInThisAssmebly = Assembly
                        .GetExecutingAssembly()
                        .GetTypes()
                        .Where(type => type.GetCustomAttribute<BehaviorAttribute>() != null)
                        .ToArray();

                    List<Type> validBehaviorTypes = new List<Type>();

                    if (behaviorsInThisAssmebly.Length > 0)
                    {
                        for (int i = 0; i < thisNpcBehaviors.Length; i++)
                        {
                            var behavior = thisNpcBehaviors[i];
                            var behaviorScriptName = behavior.Script;
                            var behaviorTypeWithScriptName
                                = behaviorsInThisAssmebly.First(b => b.GetCustomAttribute<BehaviorAttribute>().Id.ToLower().Equals(behaviorScriptName));

                            if (behaviorTypeWithScriptName != null)
                                validBehaviorTypes.Add(behaviorTypeWithScriptName);
                        }
                    }

                    tiledProcessorForMapId.AddCreature(newNpcEntity, true);

                    for (int i = 0; i < validBehaviorTypes.Count; i++)
                    {
                        var behaviorTypeToInit = validBehaviorTypes[i];
                        controllerForNpc.AddBehavior((IBehavior)Activator.CreateInstance(behaviorTypeToInit));
                    }

                    var allPlayersInProc = tiledProcessorForMapId.Creatures.Where(creature => creature.HasComponent<WorldSessionComponent>()).ToArray();

                    foreach (var player in allPlayersInProc)
                        Program.SendSerializable(player.Name, new RealmClient_CreateNPC() { Metadata = serializedNpc });

                    return newNpcEntity;
                }
                return null;
            }
        }
    }
}
