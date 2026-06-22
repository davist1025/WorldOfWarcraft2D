using LiteNetLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xna.Framework;
using MySqlX.XDevAPI;
using Nez;
using Nez.ECS.Headless;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Database.Models.Realm.Character;
using WoW.Database.Models.Realm.Chat;
using WoW.Framework.Logging;
using WoW.Network.Objects;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using WoW.Realmserver.Components;
using WoW.Realmserver.Components.Behavior;
using WoW.Realmserver.Content;
using WoW.Realmserver.Data;
using static WoW.Framework.Utils;

namespace WoW.Realmserver.Network
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
                bool characterExists = ctx.Characters.Any(c => c.Name.ToLower().Equals(characterData.Name));
                bool characterNameIsRestricted = ctx.CharacterNameFilters
                    .Any(c => c.NameOrPhrase.ToLower().Equals(characterData.Name) || characterData.Name.ToLower().StartsWith(c.NameOrPhrase.ToLower()));

                RealmClient_CreateCharacter.Result creationResult = RealmClient_CreateCharacter.Result.NameInUse;
                WorldSessionComponent session = (peer.Tag as Entity).GetComponent<WorldSessionComponent>();

                if (characterNameIsRestricted)
                    creationResult = RealmClient_CreateCharacter.Result.NameBanned;

                if (!characterExists && creationResult != RealmClient_CreateCharacter.Result.NameBanned)
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

                    var racialSpawnLocation = ctx.RaceSpawns.Where(raceSpawn => raceSpawn.RaceId == characterData.RaceId).FirstOrDefault();
                    string mapId = racialSpawnLocation.MapId;
                    Vector2 mapPosition = new Vector2(racialSpawnLocation.X, racialSpawnLocation.Y);

                    // todo: check for max character count.

                    var newCharacter = new PlayerCharacter()
                    {
                        AccountId = session.Account.Id,
                        CharacterId = (lastCharacterId + 1),
                        Name = characterData.Name.ToUpper(),
                        RaceId = characterData.RaceId,
                        HairId = characterData.HairId,
                        MapId = mapId,
                        XPosition = mapPosition.X,
                        YPosition = mapPosition.Y,
                        Direction = 3
                    };
                    ctx.Add(newCharacter);
                    ctx.SaveChanges();

                    Logger.Print($"Account (id={session.Account.Id}) has created a new character ({newCharacter.Name}).", LogEntryType.Network);
                }

                Program.Send(peer, new RealmClient_CreateCharacter() { CreationResult = creationResult });

                if (creationResult == RealmClient_CreateCharacter.Result.Success)
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
                    ctx.Characters
                        .Where(character => character.AccountId == session.Account.Id)
                        .Where(character => character.CharacterId == characterData.CharacterId)
                        .ExecuteDelete();

                    //RelationalQueryableExtensions
                    //    .ExecuteDelete(
                    //        ctx.Characters
                    //            .Where(account => account.AccountId == session.Account.Id)
                    //            .Where(character => character.CharacterId == characterData.CharacterId));
                }

                if (isSuccess)
                {
                    Logger.Print($"Account (id={session.Account.Id}) has deleted character (id={characterData.CharacterId})", LogEntryType.Network);
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
            var processorComponents = CoreHeadless.Scene.FindComponentsOfType<TiledMapProcessor>();

            using (var ctx = new RealmContext())
            {
                var activeCharacter = ctx.Characters
                    .Where(a => a.AccountId == thisSession.Account.Id)
                    .FirstOrDefault(c => c.CharacterId == join.LocalCharacterId);

                thisSession.Character = activeCharacter;
            }
            thisEntity.SetPosition(thisSession.Character.XPosition, thisSession.Character.YPosition);
            thisSession.InitializeGameComponents();
            //var collider = thisEntity.GetComponent<CircleCollider>();

            // add this entity to the matching processor.
            var mapProcessor = processorComponents.Find(processor => processor.Map.Properties["id"].ToLower().Equals(thisSession.Character.MapId));
            mapProcessor.AddCreature(thisEntity);

            // let the client create their local player object.
            Program.Send(peer, new RealmClient_CreateLocalPlayer()
            {
                WorldId = thisEntity.Name,
                Name = thisSession.Character.Name,
                RaceId = thisSession.Character.RaceId,
                HairId = thisSession.Character.HairId,
                MapId = thisSession.Character.MapId,
                ZoneX = thisSession.Character.XPosition,
                ZoneY = thisSession.Character.YPosition,
                Direction = thisSession.Character.Direction
            });
            Logger.Print($"Character ({thisSession.Character.Name}) is entering the game world.", LogEntryType.Network);

            var allSessionsExceptThis = Program
                .Scene
                .FindComponentsOfType<WorldSessionComponent>()
                .Where(session => session.Account.Id != thisSession.Account.Id)
                .ToList();

            // send this player to all players.
            // even if these players aren't on the same map, all clients still need some knowledge of each player.
            Program.SendToExcept(thisEntity.Name, new RealmClient_CreateNetPlayer()
            {
                WorldId = thisEntity.Name,
                Name = thisSession.Character.Name,
                RaceId = thisSession.Character.RaceId,
                HairId = thisSession.Character.HairId,
                MapId = thisSession.Character.MapId,
                ZoneX = thisSession.Character.XPosition,
                ZoneY = thisSession.Character.YPosition,
                Direction = thisSession.Character.Direction
            });

            // send all players to this player.
            for (int i = 0; i < allSessionsExceptThis.Count; i++)
            {
                var otherSession = allSessionsExceptThis[i];
                // todo: crash if two players are on the login/realm screen together.
                Program.SendTo(thisEntity.Name, new RealmClient_CreateNetPlayer()
                {
                    WorldId = otherSession.Entity.Name,
                    Name = otherSession.Character.Name,
                    RaceId = otherSession.Character.RaceId,
                    HairId = otherSession.Character.HairId,
                    MapId = otherSession.Character.MapId,
                    ZoneX = otherSession.Entity.Position.X,
                    ZoneY = otherSession.Entity.Position.Y,
                    Direction = otherSession.Character.Direction
                });
            }

            var npcsOnMap = mapProcessor.Creatures.Where(c => c.HasComponent<NpcControllerComponent>()).ToList();

            // send all npcs to this player.
            for (int i = 0; i < npcsOnMap.Count; i++)
            {
                var npcData = npcsOnMap[i];
                var component = npcData.GetComponent<NpcControllerComponent>();
                var newNpcPacket = new RealmClient_CreateNPC() { Metadata = component.Metadata };

                Program.SendSerializable(peer, newNpcPacket);
            }

            // tells the client they can enter the world.
            Program.SendTo(thisEntity.Name, new RealmClient_EnterWorld()
            {
                MovementSpeed = int.Parse(ConfigurationManager.AppSettings["default_player_movement_speed"]),
                MOTD = "Welcome to the official PTR for the WoW Pixel Project. Enjoy your stay!"
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

            session.QueueMovementUpdate(new ClientMovementUpdate(movement.VelocityX, movement.VelocityY, movement.DeltaTime, movement.Sequence));

        }

        /// <summary>
        /// Invoked when the player sends a new chat message.
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="peer"></param>
        public static void OnPlayerChat(ChatMessageObject newChat, NetPeer peer)
        {
            WorldSessionComponent session = (peer.Tag as Entity).GetComponent<WorldSessionComponent>();

            string unformattedMessage = newChat.Input;
            ChatChannelType channel = newChat.Channel;

            if (unformattedMessage.StartsWith("."))
            {
                string[] userCommandParts = null;
                string removedCommandIdentifier = unformattedMessage.Substring(1);

                // handle a command with arguments.
                if (removedCommandIdentifier.Contains(" "))
                {
                    using (RealmContext rctx = new RealmContext())
                    {
                        userCommandParts = removedCommandIdentifier.Split(' ');
                        string commandName = userCommandParts[0];

                        Logger.Print($"Character ({session.Character.Name}) is attempting to access command ({commandName}).", LogEntryType.Network);

                        ChatCommand commandInDb = rctx.Commands
                            .Single(command => 
                                command.Name.ToLower().Equals(commandName));

                        // todo: fix account security check.
                        if (commandInDb != null && (int)session.Account.Security >= commandInDb.Security)
                        {
                            string commandHandlerId = commandInDb.HandlerId;

                            // handle a child command based on argument 1 and the parent commands' id.
                            if (string.IsNullOrEmpty(commandHandlerId) && rctx.ChildCommands.Any(child => child.ParentId == commandInDb.Id))
                            {
                                ChatCommandChild[] childCommandsForParent = rctx.ChildCommands.Where(
                                    child =>
                                        child.ParentId == commandInDb.Id).ToArray();
                                string childCommandName = userCommandParts[1];
                                ChatCommandChild thisChild = childCommandsForParent.Single(child => child.Name.ToLower().Equals(childCommandName));

                                // process this child.
                                if (thisChild != null && (int)session.Account.Security >= thisChild.Security)
                                    ExecuteCommand(session, peer, thisChild.HandlerId, userCommandParts.Skip(2).ToArray());
                            }
                            else if (!string.IsNullOrEmpty(commandHandlerId)) // if there's a handler for the topmost command, process first and ignore any children.
                                ExecuteCommand(session, peer, commandHandlerId, userCommandParts.Skip(1).ToArray());
                        }
                    }
                }
            }

            if (unformattedMessage.StartsWith("/"))
            {
                var chatCommand = unformattedMessage.Substring(1);

                if (chatCommand.Contains(" ")) { } // todo: process chat commands with arguments (i.e: /afk im away)

                // process a single command.
                chatCommand = chatCommand.Trim();

                switch (chatCommand.ToLower())
                {
                    case "who":
                        // Sends a list of all online characters.
                        // Temporarily, GMs are included.

                        // todo: grab level, location, etc
                        SendWhoList(peer);
                        // todo: send a packet to the client.
                        // this packet will tell the client to open the Who gui if it isn't already, the data will be received first.
                        break;
                }
            }

            if (!unformattedMessage.StartsWith(".") && !unformattedMessage.StartsWith("/"))
            {
                string verbage =
                    (channel == ChatChannelType.Say) ? "says:" :
                    (channel == ChatChannelType.Yell) ? "yells:" :
                    (channel == ChatChannelType.World) ? "[world]" :
                    $"{session.Character.Name} says: {unformattedMessage}";
                    // todo: whispers, support (gm chat), etc.
                    // todo: add range for certain channels (ex: say, yell)

                string formattedMessage = "";

                if (verbage.Contains("["))
                    formattedMessage = $"{verbage} {session.Character.Name}: {unformattedMessage}";
                else
                    formattedMessage = $"{session.Character.Name} {verbage} {unformattedMessage}";

                ChatChannelType flags = ChatChannelType.Say;

                var chatPacket = new ChatMessageObject()
                {
                    Channel = flags,
                    Input = formattedMessage
                };
                Program.SendToAll(chatPacket);
            }
        }

        /// <summary>
        /// Execute a handler by string.
        /// 
        /// This function will use Reflection to find a static method that implements an attribute.
        /// </summary>
        /// <param name="bySession"></param>
        /// <param name="peer"></param>
        /// <param name="commandHandlerId"></param>
        /// <param name="args"></param>
        private static void ExecuteCommand(WorldSessionComponent bySession, NetPeer peer, string commandHandlerId, string[] args)
        {
            var handlerFunc = typeof(CommandHandler)
                .GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(func => func.GetAttribute<CommandHandlerAttribute>() != null)
                .Where(func => func.GetAttribute<CommandHandlerAttribute>().Id.ToLower().Equals(commandHandlerId.ToLower())).Single();

            handlerFunc?.Invoke(null, new object[] { args, bySession, peer });
        }

        public static void OnTabTargetRequest(NetPeer peer)
        {
            Entity playerEntity = (peer.Tag as Entity);
            WorldSessionComponent session = playerEntity.GetComponent<WorldSessionComponent>();

            // todo: add different target types for tabbing to (player, aggressive NPCs, etc)
            // i.e: check for NPCs which have the "IsAggressive" flag, hostile players, etc.
            // currently, only NPCs have a trigger for this. 
            if (session.AvailableTargets.Count > 0)
            {
                session.TargetIndex += 1;

                if (session.TargetIndex > session.AvailableTargets.Count - 1)
                    session.TargetIndex = 0;

                var newTarget = session.AvailableTargets[session.TargetIndex];
                var controller = newTarget.GetComponent<NpcControllerComponent>();

                for (int i = 0; i < controller.Behaviors.Count; i++)
                    controller.Behaviors[i].OnTargeted(session);

                Program.Send(peer, new RealmClient_SetTarget() { WorldId = controller.Metadata.Uid });
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
            Logger.Print($"Session ({transfer.SessionId}) is attempting to connect to the realm.", LogEntryType.Network);

            Program.PendingPlayers.Enqueue(new PendingPlayer(transfer.SessionId, peer));
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
                Logger.Print($"Player ({session.Character.Name}) has left the game world.", LogEntryType.Network);

                // save world position.
                if (session.Character != null)
                {
                    using (var ctx = new RealmContext())
                    {
                        // todo: test if session.Character is tracked after setting the reference.
                        // Could just do SaveChanges() here?

                        ctx.Characters
                            .Where(character => character.CharacterId == session.Character.CharacterId && character.AccountId == session.Account.Id)
                            .ExecuteUpdate(characterProp => characterProp
                                .SetProperty(characterProp => characterProp.XPosition, session.Entity.Position.X)
                                .SetProperty(characterProp => characterProp.YPosition, session.Entity.Position.Y)
                                .SetProperty(characterProp => characterProp.MapId, session.Character.MapId)
                                .SetProperty(characterProp => characterProp.Direction, session.Character.Direction));

                        //RelationalQueryableExtensions
                        //.ExecuteUpdate(
                        //    ctx.Characters
                        //        .Where(character => character.CharacterId == session.Character.CharacterId && character.AccountId == session.Account.Id), 
                        //        setters => setters
                        //            .SetProperty(c => c.XPosition, session.Entity.Position.X)
                        //            .SetProperty(c => c.YPosition, session.Entity.Position.Y)
                        //            .SetProperty(c => c.MapId, session.Character.MapId)
                        //            .SetProperty(c => c.Direction, session.Character.Direction));
                    }

                    using (var aCtx = new AuthContext())
                    {
                        aCtx.Accounts
                            .Where(account => account.Id == session.Account.Id)
                            .ExecuteUpdate(accountProp => accountProp
                                .SetProperty(accountProp => accountProp.SessionId, "-"));

                        //RelationalQueryableExtensions
                        //.ExecuteUpdate(
                        //    aCtx.Accounts
                        //        .Where(account => account.Id == session.Account.Id),
                        //        setters => setters
                        //            .SetProperty(acc => acc.SessionId, "-"));
                    }
                }

                var processors = Program.Scene.FindComponentsOfType<TiledMapProcessor>().ToArray();
                for (int i = 0; i < processors.Length; i++)
                {
                    if (processors[i].Creatures.Remove(entity))
                        Logger.Print($"Removed ({session.Character.Name}) from Tiled processor: {processors[i].Map.Properties["id"]}", LogEntryType.Debug);
                }

                // todo: only send to players within the game world; not at character select, etc.
                // add some state manager to WorldSessionComponent.Account?
                Program.SendToExcept(entity.Name, new RealmClient_Disconnect() { Id = entity.Name, Code = DisconnectCode.Timeout }, DeliveryMethod.ReliableOrdered);
            }
        }
        #endregion

        /// <summary>
        /// Central function for sending a who list to a player.
        /// 
        /// Players can request this by refreshing their who list, doing /who or /who [name-part]
        /// </summary>
        /// <param name="accountOwner"></param>
        private static void SendWhoList(NetPeer accountOwner)
        {
            var entity = accountOwner.Tag as Entity;
            string[] onlineCharacters = Program.Scene.FindComponentsOfType<WorldSessionComponent>().Select(session => session.Character.Name).ToArray();

            RealmClient_WhoCommand whoPacket = new RealmClient_WhoCommand()
            {
                Characters = onlineCharacters
            };

            Program.SendTo(entity.Name, whoPacket);

        }

        /// <summary>
        /// Sends the character list to a user.
        /// 
        /// Invoked from most character packets sent by the client.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="accountOwner"></param>
        public static void SendCharactersTo(int accountId, NetPeer accountOwner)
        {
            List<CharacterMetadataObject> characters = new List<CharacterMetadataObject>();

            using (var ctx = new RealmContext())
            {
                foreach (var character in ctx.Characters.Where(a => a.AccountId == accountId))
                    characters.Add(new CharacterMetadataObject(character.CharacterId, character.Name, (ActorRaceType)character.RaceId, character.HairId, mapId: character.MapId));
            }
            Logger.Print($"Sending {characters.Count} characters to account (id={accountId}).", LogEntryType.Network);

            Program.SendSerializable(accountOwner, new RealmClient_PlayerCharacters() { Characters = characters });
        }
    }
}
