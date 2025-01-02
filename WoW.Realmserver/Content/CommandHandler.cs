using LiteNetLib;
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
                    RemoteNPC serializedNpc = new RemoteNPC()
                    {
                        Name = npc.Name,
                        Flags = npc.FlagType,
                        Level = npc.Level
                    };

                    RealmClient_CreateNPC newNpcPacket = new RealmClient_CreateNPC()
                    {
                        Data = serializedNpc
                    };

                    Program.SendSerializable(peer, newNpcPacket);
                }
            }
        }
    }
}
