using LiteNetLib;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;
using WoW.Client.Shared.Realm;
using WoW.Realmserver.Components;
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

                    var newNpEntity = Program.Scene.CreateEntity($"{npc.Id}{npc.Name}{Nez.Random.NextInt(34000)}", session.Entity.Transform.Position);
                    
                    RemoteNPC serializedNpc = new RemoteNPC()
                    {
                        WorldId = Guid.NewGuid().ToString(),
                        Name = npc.Name,
                        Flags = npc.FlagType,
                        Level = npc.Level,
                        X = session.Entity.Transform.Position.X,
                        Y = session.Entity.Transform.Position.Y
                    };

                    newNpEntity.AddComponent(new NpcControllerComponent()
                    {
                        Data = serializedNpc
                    });

                    RealmClient_CreateNPC newNpcPacket = new RealmClient_CreateNPC()
                    {
                        Data = serializedNpc
                    };

                    Program.SendSerializableToAll(newNpcPacket);
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

            RealmClient_Chat serverMessage = new RealmClient_Chat()
            {
                FromWorldId = "server",
                Message = fullMsg
            };
            Program.SendToAll(serverMessage);
        }
    }
}
