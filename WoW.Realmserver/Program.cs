using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using System.Diagnostics;
using System.Net;
using WoW.Client.Shared;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Realm;
using WoW.Client.Shared.Serializable;
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

        private World _world;

        public static float DeltaTime = 0f;

        private Dictionary<string, NetPeer> _transferSessions = new Dictionary<string, NetPeer>();

        public Program()
        {
            Console.Title = "Realmserver";
            IsFixedTimeStep = true;

            _world = new World();
            _netProcessor = new NetPacketProcessor();

            _netEventListener = new EventBasedNetListener();
                _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.PeerConnectedEvent += (peer) => { };

            _netProcessor.SubscribeReusable<ClientRealm_Movement, NetPeer>((movement, peer) =>
            {
                var entity = peer.Tag as EntityHeadless;
                var session = entity.GetComponent<WorldSession>();

                session.InputUpdates.Enqueue(new Vector2(movement.X, movement.Y));
            });

            _netProcessor.SubscribeReusable<ClientRealm_Chat, NetPeer>((message, peer) =>
            {
                // verify the message; check for invalid characters; check for command usage.
                var entity = peer.Tag as EntityHeadless;
                var session = entity.GetComponent<WorldSession>();

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

                    WorldSession newSession = new WorldSession(session.User);
                    EntityHeadless newEntity = _world.CreateEntity(newSession.Account.SessionId.ToUpper());
                    newEntity.AddComponent(newSession);
                    sessionPeer.Tag = newEntity;

                    Console.WriteLine($"Received verification for: {session.User.SessionId} @ endpoint: {peer.EndPoint}");

                    // get all characters for this user.

                    using (var ctx = new RealmContext())
                    {
                        PlayerCharacter[] characters = ctx.Characters.Where(c => c.AccountId == session.User.Id).ToArray();
                        Console.WriteLine($"Sending {characters.Length} to {session.User.SessionId}...");

                        List<SerializableCharacter> serializableCharacters = new List<SerializableCharacter>();
                        for (int i = 0; i < characters.Length; i++)
                        {
                            var character = characters[i];
                            serializableCharacters.Add(new SerializableCharacter(character.CharacterId,character.GuildId, character.Name, character.Level, character.Class));
                        }

                        SendSerializable(sessionPeer, new RealmClient_CharacterList() { Characters = serializableCharacters });
                    }

                    //UserCharacter[] characters = Database.GetCharactersByAccountId(session.User.Id);
                    //Console.WriteLine($"Sending {characters.Length} characters to {session.User.SessionId}...");

                    //SendSerializable(sessionPeer, new RealmClient_CharacterList() { Characters = characters.ToList() });
                }
            });

            _netProcessor.SubscribeReusable<ClientRealm_TransferWorld, NetPeer>((transfer, peer) =>
            {
                EntityHeadless entity = peer.Tag as EntityHeadless;
                WorldSession session = entity.GetComponent<WorldSession>();

                // todo: re-implement player world transfer.
                using (var ctx = new RealmContext())
                {
                    var playingCharacter = ctx.Characters
                        .Where(c => c.AccountId == session.Account.Id)
                        .FirstOrDefault(c => c.CharacterId == transfer.LocalCharacterId);

                    if (playingCharacter != null)
                    {
                        session.Character = playingCharacter;

                        SerializableCharacter serializedCharacter = new SerializableCharacter(
                                playingCharacter.CharacterId,
                                playingCharacter.GuildId,
                                playingCharacter.Name,
                                playingCharacter.Level,
                                playingCharacter.Class);

                        for (int i = 0; i < _netManager.ConnectedPeersCount; i++)
                        {
                            var onlinePeer = _netManager.ConnectedPeerList[i];
                            var peerEntity = onlinePeer.Tag as EntityHeadless;
                            var otherSession = peerEntity.GetComponent<WorldSession>();
                            var otherCharacter = otherSession.Character;

                            // send all players to the new player.
                            _netProcessor.Send(peer, new RealmClient_EntityCreate()
                            {
                                EntityType = WorldEntityType.Player,
                                Id = otherSession.Account.SessionId,
                                X = otherSession.WorldPosition.X,
                                Y = otherSession.WorldPosition.Y
                            }, DeliveryMethod.ReliableOrdered);

                            SerializableCharacter serializedOtherCharacter = new SerializableCharacter(
                                otherCharacter.CharacterId, 
                                otherCharacter.GuildId,
                                otherCharacter.Name, 
                                otherCharacter.Level, 
                                otherCharacter.Class);
                            SendSerializable(peer, new RealmClient_Connect() { Id = otherSession.Account.SessionId, PlayerCharacter = serializedOtherCharacter });


                            _netProcessor.Send(onlinePeer, new RealmClient_EntityCreate()
                            {
                                EntityType = WorldEntityType.Player,
                                Id = session.Account.SessionId,
                                X = session.WorldPosition.X,
                                Y = session.WorldPosition.Y
                            }, DeliveryMethod.ReliableOrdered);

                            SendSerializable(onlinePeer, new RealmClient_Connect() { Id = otherSession.Account.SessionId, PlayerCharacter = serializedCharacter });

                            //_netProcessor.Send(peer, new RealmClient_Connect({ Id})
                            // todo: send connect packet to new peer for the above entity.
                        }
                    }
                }

                //var transferCharacter = Database.GetCharactersByAccountId(session.Account.Id).Where(c => c.LocalCharacterId == transfer.LocalCharacterId).Single();

                //if (transferCharacter != null)
                //{
                //    session.ActiveCharacterId = transferCharacter.LocalCharacterId;

                //    foreach (var onlinePeer in _netManager.ConnectedPeerList)
                //    {
                //        if (onlinePeer != peer)
                //        {
                //            var onlinePeerEntity = onlinePeer.Tag as EntityHeadless;
                //            var otherSession = onlinePeerEntity.GetComponent<WorldSession>();
                //            var thisPeerEntity = peer.Tag as EntityHeadless;

                //            // send all players to the new player.

                //            // alert the new client to create a new entity.
                //            _netProcessor.Send(peer,
                //                new RealmClient_EntityCreate()
                //                {
                //                    EntityType = WorldEntityType.Player,
                //                    Id = otherSession.Account.SessionId,
                //                    X = otherSession.WorldPosition.X,
                //                    Y = otherSession.WorldPosition.Y,
                //                }, DeliveryMethod.ReliableOrdered);

                //            _netProcessor.Send(peer,
                //                new RealmClient_Connect()
                //                {
                //                    Id = otherSession.Account.SessionId,
                //                    // todo: should we just send the whole character object?
                //                    Name = Database.GetCharacterByIdOnAccount(otherSession.Account.Id, otherSession.ActiveCharacterId).Name
                //                }, DeliveryMethod.ReliableOrdered);

                //            // send the new peer to all online players.
                //            _netProcessor.Send(onlinePeer,
                //                new RealmClient_EntityCreate()
                //                {
                //                    EntityType = WorldEntityType.Player,
                //                    Id = session.Account.SessionId,
                //                    X = session.WorldPosition.X,
                //                    Y = session.WorldPosition.Y,
                //                }, DeliveryMethod.ReliableOrdered);

                //            _netProcessor.Send(onlinePeer,
                //                new RealmClient_Connect()
                //                {
                //                    Id = session.Account.SessionId,
                //                    Name = Database.GetCharacterByIdOnAccount(session.Account.Id, session.ActiveCharacterId).Name
                //                }, DeliveryMethod.ReliableOrdered);
                //        }
                //        // todo: send all character data to the player to initialize their player object controller.
                //        // how should inventory be handled? currently "UserCharacter" is passed as an object directly to the game client, which contains the name, level, etc of the character.
                //        // in theory, this object should contain the inventory information but i dont think the player should have access to that at the realm screen(?).

                //        // todo: send this character to all online players

                //        // check for name change flag, etc.
                //    }
                //}
            });

            _netEventListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader, peer);
            _netEventListener.PeerDisconnectedEvent += (peer, reason) =>
            {
                var entity = peer.Tag as EntityHeadless;

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

            _world.Update();
        }

        private static void Send<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(peer, packet, delivery);

        private static void SendSerializable<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => _netProcessor.SendNetSerializable(peer, packet, delivery);

        public static void SendToExcept<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            var peersExcept = _netManager.ConnectedPeerList.Where(p => !(p.Tag as EntityHeadless).Name.Equals(gObjectId)).ToArray();

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
        /// Generates a global identifier for an active game object.
        /// These are shared from client/server to efficiently ID an existing object.
        /// </summary>
        /// <returns></returns>
        public static string GenerateGObjectId()
            => Guid.NewGuid().ToString().Replace("-", ""); // using guid here is probably not the best idea as the amount of "unique" id's depends on hardware.

        static void Main(string[] args)
            => new Program();
    }
}
