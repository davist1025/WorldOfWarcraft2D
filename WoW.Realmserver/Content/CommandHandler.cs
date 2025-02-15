using LiteNetLib;
using Microsoft.Xna.Framework;
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

namespace WoW.Realmserver.Content
{
    /// <summary>
    /// Signifies that a function is a command handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandHandlerAttribute : Attribute
    {
        public string Id { get; init; }

        public CommandHandlerAttribute(string id)
            => Id = id;
    }

    /// <summary>
    /// Contains all of the core functions for handling commands.
    /// </summary>
    public class CommandHandler
    {
        [CommandHandler("NpcCommand_Add")]
        public static void NpcCommand_Add(string[] commandParams, WorldSessionComponent session, NetPeer peer)
        {
            // X, Y params should be optional.
            // todo: create NPC.
            /*
             * The server should create an Entity with a unique name that corresponds to the id/name of the given NPC and set it to the position of the entity if X, Y is empty.
             * The server should then create a Controller component for the NPC to run AI routines, etc.
             * Create a serializable NPC object and form a "CreateNPC" packet for the player(s).
             * Send the NPC to all players within the map the NPC has been created.
             * 
             */
            int npcId = Convert.ToInt32(commandParams[0]);

            using (var ctx = new RealmContext())
            {
                if (ctx.NPCs.Any(npc => npc.Id == npcId))
                {
                    NonPlayerCharacter npc = ctx.NPCs.First(npc => npc.Id == npcId);
                    NonPlayerCharacterBehavior[] behaviors = ctx.Behaviors.Where(behavior => behavior.NpcId == npc.Id).ToArray();
                    Entity npcEntity = Program.Scene.CreateEntity(Guid.NewGuid().ToString(), session.Entity.Transform.Position);
                    npcEntity.Tag = (int)EntityType.NPC;

                    NpcMetadata serializedNpc = new NpcMetadata()
                    {
                        WorldId = npcEntity.Name,
                        Name = npc.Name,
                        ModelId = npc.ModelId,
                        Flags = npc.FlagType,
                        Level = npc.Level,
                        MapId = session.Character.MapId,
                        X = session.Entity.Transform.Position.X,
                        Y = session.Entity.Transform.Position.Y
                    };

                    var processorOfRecipient = Program.Scene
                        .FindComponentsOfType<TiledMapProcessor>()
                        .Single(proc => proc.Map.Properties["id"].ToLower().Equals(session.Character.MapId.ToLower()));

                    if (processorOfRecipient != null)
                    {
                        var npcController = npcEntity.AddComponent(new NpcControllerComponent()
                        {
                            Metadata = serializedNpc
                        });

                        // todo: look for external scripts, too!
                        var behaviorAttributeObjects = Assembly
                            .GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.GetCustomAttribute<BehaviorAttribute>() != null)
                            .ToArray();
                        List<Type> validBehaviorTypes = new List<Type>();

                        if (behaviorAttributeObjects.Length > 0)
                        {
                            for (int i = 0; i < behaviors.Length; i++)
                            {
                                var behavior = behaviors[i];
                                var behaviorScriptName = behavior.Script;
                                var behaviorTypeWithScriptName
                                    = behaviorAttributeObjects.First(b => b.GetCustomAttribute<BehaviorAttribute>().Id.ToLower().Equals(behaviorScriptName));

                                if (behaviorTypeWithScriptName != null)
                                    validBehaviorTypes.Add(behaviorTypeWithScriptName);
                            }
                        }

                        for (int i = 0; i < validBehaviorTypes.Count; i++)
                        {
                            var behaviorTypeToInit = validBehaviorTypes[i];
                            npcController.AddBehavior((IBehavior)Activator.CreateInstance(behaviorTypeToInit));
                        }

                        processorOfRecipient.AddCreature(npcEntity, true);

                        var allPlayersInProc = processorOfRecipient.Creatures.Where(creature => creature.HasComponent<WorldSessionComponent>()).ToArray();

                        foreach (var player in allPlayersInProc)
                            Program.SendSerializable(player.Name, new RealmClient_CreateNPC() { Metadata = serializedNpc });
                    }
                }
            }
        }

        [CommandHandler("ServerCommand_SendMessage")]
        public static void ServerCommand_SendMessage(string[] message, WorldSessionComponent session, NetPeer peer)
        {
            string fullMsg = "";

            for (int i = 0; i < message.Length; i++)
            {
                var part = message[i];

                if (i == message.Length - 1)
                    fullMsg += part;
                else
                    fullMsg += $"{part} ";
            }

            // todo: re-implement server messages!
            //RealmClient_Chat serverMessage = new RealmClient_Chat();
            //serverMessage.Channel = ChatChannelType.Server;

            //Program.SendToAll(serverMessage);
        }

        /// <summary>
        /// Summons the given Character to this player./>
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="session"></param>
        /// <param name="peer"></param>
        [CommandHandler("PlayerActionCommand_Summon")]
        public static void PlayerActionCommand_Summon(string[] data, WorldSessionComponent session, NetPeer peer)
        {
            string characterName = "";

            if (data.Length > 1)
            {
                Console.WriteLine($"This command accepts a single argument!");
                return;
            }

            characterName = data[0];

            // todo: crash here if none are found. catch this exception or determine the return value and handle accordingly.
            var characterToSummon = Program.Scene
                .FindComponentsOfType<WorldSessionComponent>()
                .Where(s => s.Account.Id != session.Account.Id && s.Character.Name.ToLower().Equals(characterName.ToLower()))
                .Single();

            if (characterToSummon != null)
            {
                Program.SendToAll(new RealmClient_Teleport()
                {
                    WorldId = characterToSummon.Entity.Name,
                    MapId = session.Character.MapId,
                    X = session.Entity.Position.X,
                    Y = session.Entity.Position.Y
                });

                var allMapProcessors = Program.Scene.FindComponentsOfType<TiledMapProcessor>();
                var thisProcessor = allMapProcessors.Where(processor => processor.Creatures.Contains(characterToSummon.Entity)).FirstOrDefault();

                if (thisProcessor != null)
                {
                    thisProcessor.Creatures.Remove(characterToSummon.Entity);

                    // set the new tiled processor for the character being summoned.
                    var newProcessor = allMapProcessors.Find(p => p.Map.Properties["id"].ToLower().Equals(session.Character.MapId));
                    newProcessor.AddCreature(characterToSummon.Entity);

                    characterToSummon.Entity.Position = new Vector2(session.Entity.Position.X, session.Entity.Position.Y);

                    // this feels crash-prone.
                    var playersInNewProcessor = newProcessor.Creatures
                        .Where(creature => creature.HasComponent<WorldSessionComponent>() && !creature.Name.ToLower().Equals(characterToSummon.Entity.Name.ToLower()))
                        .ToArray();

                    // send the current positions of all players in the summoned map since we don't send input updates outside of the players' map.
                    foreach (var player in playersInNewProcessor)
                    {
                        Program.SendTo(characterToSummon.Entity.Name,
                            new RealmClient_NetPositionInputUpdate()
                            {
                                Id = player.Name,
                                ResultX = player.Transform.Position.X,
                                ResultY = player.Transform.Position.Y,
                                MovementX = 0f,
                                MovementY = 0f,
                                IsTeleportUpdate = true
                            }, DeliveryMethod.ReliableOrdered);
                    }

                    // send all NPCs to this summoned player.
                    var npcsInNewProcessor = newProcessor.Creatures.Where(creature => creature.HasComponent<NpcControllerComponent>()).ToArray();

                    foreach (var npc in npcsInNewProcessor)
                    {
                        var component = npc.GetComponent<NpcControllerComponent>();

                        Program.SendSerializable(characterToSummon.Entity.Name,
                            new RealmClient_CreateNPC()
                            {
                                Metadata = component.Metadata
                            });
                    }
                }
            }
        }

        [CommandHandler("PlayerActionCommand_TestSec")]
        public static void PlayerActionCommand_TestSecurity(string[] data, WorldSessionComponent session, NetPeer peer)
        {
            Console.WriteLine($"{session.Character.Name} has valid security for this command!");
        }
    }
}
