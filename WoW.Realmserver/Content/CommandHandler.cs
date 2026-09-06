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
using WoW.Realmserver.Components;
using WoW.Realmserver.Components.Behavior;
using WoW.Framework;
using WoW.Framework.Network.Container;

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
        public static void NpcCommand_Add(string[] commandParams, SessionComponent session, NetPeer peer)
        {
            // X, Y params should be optional.
            /*
             * The server should create an Entity with a unique name that corresponds to the id/name of the given NPC and set it to the position of the entity if X, Y is empty.
             * The server should then create a Controller component for the NPC to run AI routines, etc.
             * Create a serializable NPC object and form a "CreateNPC" packet for the player(s).
             * Send the NPC to all players within the map the NPC has been created.
             * 
             */
            int npcId = Convert.ToInt32(commandParams[0]);

                // todo: send invalid id response.
        }

        [CommandHandler("ServerCommand_SendMessage")]
        public static void ServerCommand_SendMessage(string[] message, SessionComponent session, NetPeer peer)
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


            // todo: [command handler] send server message + chat message container.
            //Program.SendToAll(newServerMessage);

            // todo: re-implement server messages!
            //RealmClient_Chat serverMessage = new RealmClient_Chat();
            //serverMessage.Channel = ChatChannelType.Server;

            //Program.SendToAll(serverMessage);
        }
    }
}
