using LiteNetLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xna.Framework;
using MySqlX.XDevAPI;
using Nez;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Data;
using WoW.Client.Shared.Realm;
using WoW.Realmserver.Components;
using WoW.Realmserver.Content;
using WoW.Realmserver.DB;
using WoW.Realmserver.DB.Model;
using WoW.Server.Shared;

namespace WoW.Realmserver
{
    /// <summary>
    /// Houses all packet handlers.
    /// </summary>
    public static class PacketManager
    {
        #region Character packets
        public static void OnPlayerCreateCharacter(ClientRealm_CreateCharacter characterData, NetPeer peer)
        {
            using (var ctx = new RealmContext())
            {
                bool characterExists = ctx.Characters.Any(c => c.Name.Equals(characterData.Name.ToUpper()));

                RealmClient_CreateCharacter.Result creationResult = RealmClient_CreateCharacter.Result.NameInUse;
                WorldSessionComponent session = (peer.Tag as Entity).GetComponent<WorldSessionComponent>();

                if (!characterExists)
                {
                    creationResult = RealmClient_CreateCharacter.Result.Success;
                    var dbCharacters = ctx.Characters.Where(x => x.AccountId == session.Account.Id).ToList();
                    int lastCharacterId = 0;

                    if (dbCharacters.Count > 0)
                    {
                        lastCharacterId = dbCharacters
                            .Where(c => c.AccountId == session.Account.Id)
                            .Select(c => c.CharacterId)
                            .Max();
                    }
                    // todo: check for max character count.

                    var newCharacter = new PlayerCharacter()
                    {
                        AccountId = session.Account.Id,
                        CharacterId = (lastCharacterId + 1),
                        Name = characterData.Name.ToUpper(),
                        RaceId = characterData.RaceId,
                        HairId = characterData.HairId,
                        XPosition = 50f,
                        YPosition = 50f
                    };
                    ctx.Add(newCharacter);
                    ctx.SaveChanges();

                    Console.WriteLine($"Account ID: {session.Account.Id} has created a new character: {newCharacter.Name}");
                }

                Program.Send(peer, new RealmClient_CreateCharacter() { CreationResult = creationResult });

                SendCharactersTo(session.Account.Id, peer);
            }
        }

        public static void OnPlayerDeleteCharacter(ClientRealm_DeleteCharacter characterData, NetPeer peer)
        {
            var entity = peer.Tag as Entity;
            var session = entity?.GetComponent<WorldSessionComponent>();

            if (session != null)
            {
                bool isSuccess = false;

                using (var ctx = new RealmContext())
                {
                    isSuccess = (ctx.Characters
                        .Where(a => a.AccountId == session.Account.Id)
                        .Where(c => c.CharacterId == characterData.CharacterId)
                        .ExecuteDelete()) > 0;
                }

                if (isSuccess)
                {
                    Console.WriteLine($"Account ID: {session.Account.Id} is deleting character id: {characterData.CharacterId}");
                    SendCharactersTo(session.Account.Id, peer);
                }
            }
        }

        /// <summary>
        /// Invoked when the client requests their list of characters.
        /// 
        /// Typically sent after a player connects to the realm and creates or deletes a character.
        /// </summary>
        /// <param name="reqList"></param>
        /// <param name="peer"></param>
        public static void OnPlayerRequestCharacters(ClientRealm_RequestCharacterList reqList, NetPeer peer)
        {
            WorldSessionComponent session = (peer.Tag as Entity).GetComponent<WorldSessionComponent>();
            SendCharactersTo(session.Account.Id, peer);
        }
        #endregion

        #region World packets
        /// <summary>
        /// Invoked when the player selects a character and joins the world.
        /// 
        /// This function structures everything the client will initially need to join the world.
        /// </summary>
        /// <param name="join"></param>
        /// <param name="peer"></param>
        public static void OnPlayerJoinWorld(ClientRealm_TransferWorld join, NetPeer peer)
        {
            Entity thisEntity = peer.Tag as Entity;
            WorldSessionComponent thisSession = thisEntity.GetComponent<WorldSessionComponent>();

            using (var ctx = new RealmContext())
            {
                var activeCharacter = ctx.Characters
                    .Where(a => a.AccountId == thisSession.Account.Id)
                    .FirstOrDefault(c => c.CharacterId == join.LocalCharacterId);

                thisSession.Character = activeCharacter;
            }
            thisSession.InitializeGameComponents();

            // let the client create their local player object.
            Program.Send(peer, new RealmClient_CreateLocalPlayer()
            {
                WorldId = thisEntity.Name,
                Name = thisSession.Character.Name,
                RaceId = thisSession.Character.RaceId,
                HairId = thisSession.Character.HairId,
                MapId = "world1",
                ZoneX = thisSession.Character.XPosition,
                ZoneY = thisSession.Character.YPosition
            });
            Console.WriteLine($"{thisSession.Character.Name} is entering the world!");

            // send this player to all players.
            Program.SendToExcept(thisEntity.Name, new RealmClient_CreateNetPlayer()
            {
                WorldId = thisEntity.Name,
                Name = thisSession.Character.Name,
                RaceId = thisSession.Character.RaceId,
                HairId = thisSession.Character.HairId,
                ZoneX = thisSession.Character.XPosition,
                ZoneY = thisSession.Character.YPosition
            });

            var allSessionsExceptThis = Program.Scene.FindComponentsOfType<WorldSessionComponent>().Where(session => session.Account.Id != thisSession.Account.Id).ToList();

            // send all players to this player.
            for (int i = 0; i < allSessionsExceptThis.Count; i++)
            {
                var otherSession = allSessionsExceptThis[i];
                Program.SendTo(thisEntity.Name, new RealmClient_CreateNetPlayer()
                {
                    WorldId = otherSession.Entity.Name,
                    Name = otherSession.Character.Name,
                    RaceId = otherSession.Character.RaceId,
                    HairId = otherSession.Character.HairId,
                    ZoneX = otherSession.Entity.Position.X,
                    ZoneY = otherSession.Entity.Position.Y
                });
            }

            var allNpcs = Program.Scene.FindComponentsOfType<NpcControllerComponent>().Where(n => n.Data != null).ToList();

            // send all npcs to this player.
            for (int i = 0; i < allNpcs.Count; i++)
            {
                var npcData = allNpcs[i];
                var newNpcPacket = new RealmClient_CreateNPC() { Data = npcData.Data };

                Program.SendSerializable(peer, newNpcPacket);
            }

            // tells the client they can enter the world.
            Program.SendTo(thisEntity.Name, new RealmClient_EnterWorld()
            {
                MovementSpeed = Program.Configuration.WorldParameters["global_movement_speed"]
            });
        }

        /// <summary>
        /// Invoked when the player sends movement input.
        /// </summary>
        /// <param name="movement"></param>
        /// <param name="peer"></param>
        public static void OnPlayerMove(ClientRealm_Movement movement, NetPeer peer)
        {
            var entity = peer.Tag as Entity;
            var session = entity.GetComponent<WorldSessionComponent>();

            session.InputUpdates.Enqueue(new Vector2(movement.X, movement.Y));
        }

        /// <summary>
        /// Invoked when the player sends a new chat message.
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="peer"></param>
        public static void OnPlayerChat(ClientRealm_Chat chat, NetPeer peer)
        {
            // verify the message; check for invalid characters; check for command usage.
            var entity = peer.Tag as Entity;
            var session = entity.GetComponent<WorldSessionComponent>();

            if (chat.Message.StartsWith("."))
            {
                if (chat.Message.Contains(" "))
                {
                    string[] msgCopy = chat.Message.Substring(1).Split(" ");
                    string commandName = msgCopy[0];

                    using (var ctx = new RealmContext())
                    {
                        if (ctx.Commands.Any(c => c.Name.Equals(commandName) && c.Security == (int)session.Account.Security))
                        {
                            var command = ctx.Commands.Single(c => c.Name.Equals(commandName));
                            if (command.HandlerId == null)
                            {
                                string childCommandName = msgCopy[1];

                                if (ctx.ChildCommands.Any(c => c.Name.Equals(childCommandName) && c.Security == (int)session.Account.Security))
                                {
                                    var childCommand = ctx.ChildCommands.Single(c => c.Name.Equals(childCommandName));
                                    var childCommandHandlerId = childCommand.HandlerId;

                                    Console.WriteLine($"{session.Character.Name} is attempting to process command: '{commandName} {childCommandName}'.");

                                    var handlerFunc = typeof(CommandHandler)
                                        .GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                                        .Where(func => func.GetAttribute<CommandHandlerAttribute>() != null)
                                        .Where(func => func.GetAttribute<CommandHandlerAttribute>().Id.Equals(childCommandHandlerId))
                                        .Single();

                                    handlerFunc?.Invoke(null, new object[] { msgCopy.Skip(2).ToArray(), session, peer });
                                }
                            }
                            else
                            {
                                // todo: use parent handlerid w/ Reflection to process command.
                                // children will not be processed if a handler exists for the top-most command.
                            }
                        }
                    }
                }
            }
            else
                Program.SendToAll(new RealmClient_Chat() 
                { 
                    Id = entity.Name, 
                    Message = chat.Message 
                });
        }

        public static void OnTabTargetRequest(NetPeer peer)
        {
            Entity playerEntity = (peer.Tag as Entity);
            WorldSessionComponent session = playerEntity.GetComponent<WorldSessionComponent>();

            if (session.AvailableTargets.Count > 0)
            {
                session.TargetIndex += 1;

                if (session.TargetIndex > session.AvailableTargets.Count - 1)
                    session.TargetIndex = 0;

                var newTarget = session.AvailableTargets[session.TargetIndex];
                var controller = newTarget.GetComponent<NpcControllerComponent>();

                Program.Send(peer, new RealmClient_SetTarget() { WorldId = controller.Data.WorldId });
            }
        }
        #endregion

        #region Realm/auth packets
        /// <summary>
        /// Invoked when a player selects a realm from the Realmlist, sent from the Authserver.
        /// </summary>
        /// <param name="transfer"></param>
        /// <param name="peer"></param>
        public static void OnPlayerTransferToRealm(ClientRealm_TransferLogon transfer, NetPeer peer)
        {
            Console.WriteLine($"Received logon transfer from: {transfer.SessionId}");

            Program.TransferSessions.Add(transfer.SessionId, peer);
            Program.SendToAuthserver(new RealmAuth_SessionVerification() { SessionId = transfer.SessionId });
        }

        /// <summary>
        /// Invoked when the authserver sends back a verification response.
        /// 
        /// This occurs when the realmserver attempts to verify a user's SessionId when they connect.
        /// </summary>
        /// <param name="verification"></param>
        /// <param name="peer"></param>
        public static void OnAuthSessionVerification(AuthRealm_SessionVerification verification, NetPeer peer)
        {
            if (verification.User != null && Program.TransferSessions.ContainsKey(verification.User.SessionId))
            {
                NetPeer sessionPeer = Program.TransferSessions[verification.User.SessionId];
                Program.TransferSessions.Remove(verification.User.SessionId);

                WorldSessionComponent newSession = new WorldSessionComponent(verification.User);
                Entity newEntity = Program.Scene.CreateEntity(Guid.NewGuid().ToString());
                newEntity.Tag = (int)EntityType.NetPlayer;
                newEntity.AddComponent(newSession);
                sessionPeer.Tag = newEntity;

                Console.WriteLine($"Received verification for: {verification.User.SessionId} @ endpoint: {peer.EndPoint}");

                // get all characters for this user.

                using (var ctx = new RealmContext())
                {
                    List<RemoteCharacter> characters = new List<RemoteCharacter>();

                    foreach (var character in ctx.Characters.Where(a => a.AccountId == newSession.Account.Id))
                        characters.Add(new RemoteCharacter(character.CharacterId, character.Name, character.RaceId, character.HairId));

                    Console.WriteLine($"Sending {characters.Count} to client...");

                    Program.SendSerializable(sessionPeer, new RealmClient_PlayerCharacters() { Characters = characters });
                }
            }
        }

        /// <summary>
        /// Invoked when a client disconnects for any reason.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="disconnectInfo"></param>
        public static void OnClientDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (peer.Tag is Entity)
            {
                var entity = peer.Tag as Entity;
                var session = entity.GetComponent<WorldSessionComponent>();
                Console.WriteLine($"Account ID: {session.Account.Id} is disconnecting...");

                Program.SendToAuthserver(new RealmAuth_Disconnection() { AccountId = session.Account.Id });

                // save world position.
                if (session.Character != null)
                {
                    using (var ctx = new RealmContext())
                    {
                        // todo: test if session.Character is tracked after setting the reference.
                        // Could just do SaveChanges() here?

                        ctx.Characters
                        .Where(c => c.CharacterId == session.Character.CharacterId && c.AccountId == session.Account.Id)
                        .ExecuteUpdate(setters => setters
                            .SetProperty(c => c.XPosition, session.Entity.Position.X)
                            .SetProperty(c => c.YPosition, session.Entity.Position.Y));
                        // todo: set mapid.
                    }
                }

                // todo: only send to players within the game world; not at character select, etc.
                // add some state manager to WorldSessionComponent.Account?
                Program.SendToExcept(entity.Name, new RealmClient_Disconnect() { Id = entity.Name, Code = DisconnectCode.Timeout }, DeliveryMethod.ReliableOrdered);
            }
        }
        #endregion

        /// <summary>
        /// Sends the character list to a user.
        /// 
        /// Invoked from most character packets sent by the client.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="accountOwner"></param>
        private static void SendCharactersTo(int accountId, NetPeer accountOwner)
        {
            List<RemoteCharacter> characters = new List<RemoteCharacter>();

            using (var ctx = new RealmContext())
            {
                foreach (var character in ctx.Characters.Where(a => a.AccountId == accountId))
                    characters.Add(new RemoteCharacter(character.CharacterId, character.Name, character.RaceId, character.HairId));
            }
            Console.WriteLine($"Sending {characters.Count} to client...");

            Program.SendSerializable(accountOwner, new RealmClient_PlayerCharacters() { Characters = characters });
        }
    }
}
