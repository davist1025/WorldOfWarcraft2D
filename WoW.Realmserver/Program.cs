using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using Nez.Systems;
using Nez.Tiled;
using System.Diagnostics;
using System.Net;
using WoW.Client.Shared;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Realm;
using WoW.Client.Shared.Serializable;
using WoW.Realmserver.Components;
using WoW.Realmserver.Components.Scripts;
using WoW.Realmserver.Content;
using WoW.Realmserver.DB;
using WoW.Realmserver.DB.Model;
using WoW.Server.Shared;

namespace WoW.Realmserver
{
    internal class Program : CoreHeadless
    {
        private static NetManager _netManager;
        private static NetManager _authNetManager;
        private EventBasedNetListener _netEventListener;
        private EventBasedNetListener _authListener;
        private static NetPacketProcessor _netProcessor;

        public static WorldContentManager Content;

        public static float DeltaTime = 0f;

        private Dictionary<string, NetPeer> _transferSessions = new Dictionary<string, NetPeer>();

        public Program()
        {
            Console.Title = "Realmserver";
            TiledMapLoader.IsHeadless = true;
            Scene.IsHeadless = true;

            IsFixedTimeStep = true;
            Scene = new WorldScene();

            // hack: debug script loading code.
            var script = ScriptLoader.GetScriptFromAssembly("targeted_script");

            Content = new WorldContentManager();
            _netProcessor = new NetPacketProcessor();

            // load content.
            Content.LoadTiled();

            _netEventListener = new EventBasedNetListener();
                _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.PeerConnectedEvent += (peer) => { };

            _netProcessor.SubscribeReusable<ClientRealm_Movement, NetPeer>((movement, peer) =>
            {
                var entity = peer.Tag as Entity;
                var session = entity.GetComponent<WorldSessionComponent>();

                session.InputUpdates.Enqueue(new Vector2(movement.X, movement.Y));
            });

            _netProcessor.SubscribeReusable<ClientRealm_Chat, NetPeer>((message, peer) =>
            {
                // verify the message; check for invalid characters; check for command usage.
                var entity = peer.Tag as Entity;
                var session = entity.GetComponent<WorldSessionComponent>();

                // todo: command processing!
                //if (message.Message.StartsWith('.'))
                //{
                //    // parse the string out to get the command and arguments.
                //    // there could be multiple arguments depending on command.

                //    // exmaple: .gm on/off
                //    // .gobject create [id] [x] [y] [z]
                //    // .additem [id]
                //    // .server (general server information)
                //    // .kick [username]
                //    // .ban [username] [reason] [duration]
                //    // .ticket
                //    //      create
                //    //      delete
                //    //      view
                //    //      assign [username]
                //}
                //else
                    SendToAll(new RealmClient_Chat() { Id = entity.Name, Message = message.Message }, DeliveryMethod.ReliableOrdered);
            });

            _netProcessor.SubscribeReusable<ClientRealm_TransferLogon, NetPeer>((transfer, peer) =>
            {
                Console.WriteLine($"Received logon transfer from: {transfer.SessionId}");

                _transferSessions.Add(transfer.SessionId, peer);
                SendToAuthserver(new RealmAuth_SessionVerification() { SessionId = transfer.SessionId });
            });

            _netProcessor.SubscribeNetSerializable<AuthRealm_SessionVerification, NetPeer>((session, peer) =>
            {
                if (session.User != null && _transferSessions.ContainsKey(session.User.SessionId))
                {
                    NetPeer sessionPeer = _transferSessions[session.User.SessionId];
                    _transferSessions.Remove(session.User.SessionId);

                    WorldSessionComponent newSession = new WorldSessionComponent(session.User);
                    Entity newEntity = Scene.CreateEntity(newSession.Account.SessionId);
                    newEntity.AddComponent(newSession);
                    sessionPeer.Tag = newEntity;

                    Console.WriteLine($"Received verification for: {session.User.SessionId} @ endpoint: {peer.EndPoint}");

                    // get all characters for this user.

                    using (var ctx = new RealmContext())
                    {
                        PlayerCharacter[] characters = ctx.Characters.Where(c => c.AccountId == session.User.Id).ToArray();
                        Console.WriteLine($"Sending {characters.Length} to {session.User}...");

                        List<SerializableCharacter> serializableCharacters = new List<SerializableCharacter>();
                        for (int i = 0; i < characters.Length; i++)
                        {
                            var character = characters[i];
                            serializableCharacters.Add(new SerializableCharacter(character.CharacterId, character.RaceId, character.GuildId, character.Name, character.Level, character.Class));
                        }

                        // todo: should we still send this packet if the list is empty?
                        // suppose the client could just check the list count and if it's <1, just diaply no characters :p
                        SendSerializable(sessionPeer, new RealmClient_CharacterList() { Characters = serializableCharacters });
                    }
                }
            });

            // this is where we will send the connecting client everything they need to play.
            _netProcessor.SubscribeReusable<ClientRealm_TransferWorld, NetPeer>((transfer, peer) =>
            {
                Entity entity = peer.Tag as Entity;
                entity.Tag = (int)GameObjectType.Player;
                WorldSessionComponent session = entity.GetComponent<WorldSessionComponent>();

                using (var ctx = new RealmContext())
                {
                    var playingCharacter = ctx.Characters
                        .Where(c => c.AccountId == session.Account.Id)
                        .FirstOrDefault(c => c.CharacterId == transfer.LocalCharacterId);

                    if (playingCharacter != null)
                    {
                        session.Character = playingCharacter;
                        session.InitializeGameComponents();

                        // send the client their chosen character.
                        SerializableCharacter serializedCharacter = new SerializableCharacter(
                                playingCharacter.CharacterId,
                                playingCharacter.RaceId,
                                playingCharacter.GuildId,
                                playingCharacter.Name,
                                playingCharacter.Level,
                                playingCharacter.Class);

                        var allPeersExceptSender = _netManager.ConnectedPeerList.Where(p => p.Id != peer.Id).ToArray();
                        for (int i = 0; i < allPeersExceptSender.Length; i++)
                        {
                            NetPeer onlinePeer = allPeersExceptSender[i];
                            Entity entityForPeer = onlinePeer.Tag as Entity;
                            WorldSessionComponent sessionForEntity = entityForPeer.GetComponent<WorldSessionComponent>();

                            if (sessionForEntity.Character.MapId.Equals(session.Character.MapId, StringComparison.OrdinalIgnoreCase))
                            {
                                SerializableCharacter serializedOnlineCharacter = new SerializableCharacter(
                                sessionForEntity.Character.CharacterId,
                                sessionForEntity.Character.RaceId,
                                sessionForEntity.Character.GuildId,
                                sessionForEntity.Character.Name,
                                sessionForEntity.Character.Level,
                                sessionForEntity.Character.Class);

                                Send(peer, new RealmClient_CreateGameObject()
                                {
                                    EntityType = GameObjectType.Player,
                                    Id = sessionForEntity.Account.SessionId,
                                    X = sessionForEntity.Entity.Transform.Position.X,
                                    Y = sessionForEntity.Entity.Transform.Position.Y
                                });
                                SendSerializable(peer, new RealmClient_CreateNetPlayer()
                                {
                                    Id = sessionForEntity.Account.SessionId,
                                    PlayerCharacter = serializedOnlineCharacter
                                });

                                Send(onlinePeer, new RealmClient_CreateGameObject()
                                {
                                    EntityType = GameObjectType.Player,
                                    Id = session.Account.SessionId,
                                    X = session.Entity.Transform.Position.X,
                                    Y = session.Entity.Transform.Position.Y
                                });
                                SendSerializable(onlinePeer, new RealmClient_CreateNetPlayer()
                                {
                                    Id = session.Account.SessionId,
                                    PlayerCharacter = serializedCharacter
                                });
                            }
                        }

                        var nonPlayerCreatures = Scene.FindEntitiesWithTag((int)GameObjectType.Creature);
                        for (int i = 0; i < nonPlayerCreatures.Count; i++)
                        {
                            var creature = nonPlayerCreatures[i];
                            var gObjectComponent = creature.GetComponent<GameObjectComponent>();

                            if (gObjectComponent.MapId.Equals(session.Character.MapId, StringComparison.OrdinalIgnoreCase))
                            {
                                Send(peer, new RealmClient_CreateGameObject()
                                {
                                    EntityType = GameObjectType.Creature,
                                    Id = gObjectComponent.Id,
                                    X = creature.Transform.Position.X,
                                    Y = creature.Transform.Position.Y,
                                });

                                Send(peer, new RealmClient_CreateNetObject()
                                {
                                    Id = gObjectComponent.Id,
                                    Creature = new SerializableCreature()
                                    {
                                        Name = gObjectComponent.Creature.Name,
                                        SubName = gObjectComponent.Creature.SubName,
                                        DisplayId = gObjectComponent.Creature.DisplayId,
                                        Flags = gObjectComponent.Creature.Flags,
                                    }
                                });
                            }
                        }
                    }
                }
            });

            _netEventListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader, peer);
            _netEventListener.PeerDisconnectedEvent += (peer, reason) =>
            {
                var entity = peer.Tag as Entity;

                SendToExcept(entity.Name, new RealmClient_Disconnect() { Id = entity.Name, Code = DisconnectCode.Timeout }, DeliveryMethod.ReliableOrdered);
            };

            _netManager = new NetManager(_netEventListener);
            _netManager.Start(8080);

            _authListener = new EventBasedNetListener();
            _authListener.PeerConnectedEvent += (peer) =>
            {
                // todo: grab from config.
                SendToAuthserver(new RealmAuth_Registrar() { Name = "PTR", Ip = "127.0.0.1", Port = 8080 });
            };

            _authListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader, peer);
            _authNetManager = new NetManager(_authListener);
            _authNetManager.Start();
            _authNetManager.Connect("127.0.0.1", 8070, "");

            // authentication server will register each realmserver and give them to a connecting client.
            // client will select a realmserver, tell the authentication server which one.

            // todo: try to add a behavior plugin system for ai routines so they can be "scripted"?
            // will attempt to use CSScript and use Nez' BehaviorTree system.

            CreateCreature("debug_world_1", Vector2.Zero, rawId: "mailbox_basic");

            while (true)
            {
                _netManager.PollEvents();
                _authNetManager.PollEvents();
                Tick();
            }
        }

        public override void Update(float deltaTime)
        {
            DeltaTime = deltaTime;
            Scene.Update();
        }

        private static void Send<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(peer, packet, delivery);

        /// <summary>
        /// Used to send an object which is not readily recognized by LNL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="peer"></param>
        /// <param name="packet"></param>
        /// <param name="delivery"></param>
        private static void SendSerializable<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => _netProcessor.SendNetSerializable(peer, packet, delivery);

        public static void SendToExcept<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            var peersExcept = _netManager.ConnectedPeerList.Where(p => !(p.Tag as Entity).Name.Equals(gObjectId)).ToArray();

            for (int i = 0; i < peersExcept.Length; i++)
                Send(peersExcept[i], packet, delivery);
        }

        public static void SendToAll<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            for (int i = 0; i < _netManager.ConnectedPeersCount; i++)
                Send(_netManager.ConnectedPeerList[i], packet, delivery);
        }

        public static void SendToAuthserver<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(_authNetManager, packet, delivery);

        /// <summary>
        /// Adds a Creature to the world.
        /// </summary>
        /// <param name="creatureId"></param>
        /// <param name="mapId"></param>
        /// <param name="position"></param>
        public static void CreateCreature(string mapId, Vector2 position, int creatureId = -1, string rawId = "")
        {
            // todo: make use of "raw id" in the creature object.
            // this will be a universally unique string id that is more recognizable.
            if (!Scene.GetType().Equals(typeof(WorldScene)))
            {
                Console.WriteLine("Scene is not a WorldScene object.");
                return;
            }

            // todo: check for existing map id.
            using (var ctx = new RealmContext())
            {
                Creature creature = null;

                if (creatureId != -1)
                    creature = ctx.Creatures.Where(c => c.Id == creatureId).FirstOrDefault();
                else if (rawId != "")
                    creature = ctx.Creatures.Where(c => c.RawId == rawId).FirstOrDefault();

                if (creature == null)
                {
                    Console.WriteLine($"Unable to create an instance of this Creature; invalid id given!");
                    return;
                }

                // todo: meh, find a better way to name creature entities.
                Entity newCreature = Scene.CreateEntity($"{creature.Name}_{Nez.Random.NextInt(1000)}");
                newCreature.Tag = (int)GameObjectType.Creature;
                newCreature.AddComponent(new GameObjectComponent(mapId, creature));
            }
            // create and add the entity to the scene so they receive tick updates.
            // place the mapId somewhere on the entity so we can reference them when the player joins a map.
        }

        static void Main(string[] args)
            => new Program();
    }
}
