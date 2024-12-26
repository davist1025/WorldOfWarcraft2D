using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using Nez.Systems;
using Nez.Tiled;
using System.Diagnostics;
using System.Net;
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

        private string _hostName = "127.0.0.1";
        private int _port = 3733;

        public Program()
        {
            Console.Title = "Realmserver";
            TiledMapLoader.IsHeadless = true;
            Scene.IsHeadless = true;

            IsFixedTimeStep = true;
            Scene = new WorldScene();

            Content = new WorldContentManager();
            _netProcessor = new NetPacketProcessor();

            // load content.
            Content.LoadTiled();

            // todo: ensurecreated for testing.
            // data doesn't need to persist across test runs, and can be initialized on startup.

            _netEventListener = new EventBasedNetListener();
            _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.PeerConnectedEvent += (peer) => { };
            _netEventListener.PeerDisconnectedEvent += (peer, reason) =>
            {
                if (peer.Tag is Entity)
                {
                    var entity = peer.Tag as Entity;
                    var session = entity.GetComponent<WorldSessionComponent>();
                    Console.WriteLine($"Account ID: {session.Account.Id} is disconnecting...");

                    SendToAuthserver(new RealmAuth_Disconnection() { AccountId = session.Account.Id });

                    // save world position.
                    if (session.Character != null)
                    {
                        using (var ctx = new RealmContext())
                        {
                            ctx.Characters
                            .Where(c => c.CharacterId == session.Character.CharacterId && c.AccountId == session.Account.Id)
                            .ExecuteUpdate(setters => setters
                                .SetProperty(c => c.XPosition, session.Entity.Position.X)
                                .SetProperty(c => c.YPosition, session.Entity.Position.Y));
                            // todo: set mapid.
                        }
                    }

                    // todo: only send to players within the game world; not at character select, etc.
                    SendToExcept(entity.Name, new RealmClient_Disconnect() { Id = entity.Name, Code = DisconnectCode.Timeout }, DeliveryMethod.ReliableOrdered);
                }
            };

            _netProcessor.SubscribeReusable<ClientRealm_Movement, NetPeer>((movement, peer) =>
            {
                var entity = peer.Tag as Entity;
                var session = entity.GetComponent<WorldSessionComponent>();

                session.InputUpdates.Enqueue(new Vector2(movement.X, movement.Y));
            });

            //_netProcessor.SubscribeReusable<ClientRealm_Chat, NetPeer>((message, peer) =>
            //{
            //    // verify the message; check for invalid characters; check for command usage.
            //    var entity = peer.Tag as Entity;
            //    var session = entity.GetComponent<WorldSessionComponent>();

            //    // todo: command processing!
            //    //if (message.Message.StartsWith('.'))
            //    //{
            //    //    // parse the string out to get the command and arguments.
            //    //    // there could be multiple arguments depending on command.

            //    //    // exmaple: .gm on/off
            //    //    // .gobject create [id] [x] [y] [z]
            //    //    // .additem [id]
            //    //    // .server (general server information)
            //    //    // .kick [username]
            //    //    // .ban [username] [reason] [duration]
            //    //    // .ticket
            //    //    //      create
            //    //    //      delete
            //    //    //      view
            //    //    //      assign [username]
            //    //}
            //    //else
            //        SendToAll(new RealmClient_Chat() { Id = entity.Name, Message = message.Message }, DeliveryMethod.ReliableOrdered);
            //});

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
                        //PlayerCharacter[] characters = ctx.Characters.Where(c => c.AccountId == session.User.Id).ToArray();
                        //Console.WriteLine($"Sending {characters.Length} to {session.User}...");

                        List<RemoteCharacter> characters = new List<RemoteCharacter>();

                        // todo: get db charactes based on account id.

                        foreach (var character in ctx.Characters.Where(a => a.AccountId == newSession.Account.Id))
                            characters.Add(new RemoteCharacter(character.CharacterId, character.Name));

                        Console.WriteLine($"Sending {characters.Count} to client...");

                        SendSerializable(sessionPeer, new RealmClient_PlayerCharacters() { Characters = characters });
                    }
                }
            });

            _netProcessor.SubscribeReusable<ClientRealm_CreateCharacter, NetPeer>((request, peer) =>
            {
                using (var ctx = new RealmContext())
                {
                    bool characterExists = ctx.Characters.Any(c => c.Name.Equals(request.Name.ToUpper()));

                    RealmClient_CreateCharacter.Result creationResult = RealmClient_CreateCharacter.Result.NameInUse;
                    WorldSessionComponent session = (peer.Tag as Entity).GetComponent<WorldSessionComponent>();

                    if (!characterExists)
                    {
                        creationResult = RealmClient_CreateCharacter.Result.Success;
                        var dbCharacters = ctx.Characters.Where(x => x.AccountId == session.Account.Id).ToList();
                        int lastCharacterId = 0;

                        if (dbCharacters.Count > 0)
                        {
                            // todo: check character count per account id.
                            // this throws a "Sequence contains no elements" exception.
                            lastCharacterId = dbCharacters
                                .Where(c => c.AccountId == session.Account.Id)
                                .Select(c => c.CharacterId)
                                .Max();
                        }

                        var newCharacter = new PlayerCharacter()
                        {
                            AccountId = session.Account.Id,
                            CharacterId = (lastCharacterId + 1),
                            Name = request.Name.ToUpper(),
                            XPosition = 50f,
                            YPosition = 50f
                        };
                        ctx.Add(newCharacter);
                        ctx.SaveChanges();

                        Console.WriteLine($"Account ID: {session.Account.Id} has created a new character: {newCharacter.Name}");
                    }

                    Send(peer, new RealmClient_CreateCharacter() { CreationResult = creationResult });

                    List<RemoteCharacter> characters = new List<RemoteCharacter>();

                    foreach (var character in ctx.Characters.Where(a => a.AccountId == session.Account.Id))
                        characters.Add(new RemoteCharacter(character.CharacterId, character.Name));

                    Console.WriteLine($"Sending {characters.Count} to client...");

                    SendSerializable(peer, new RealmClient_PlayerCharacters() { Characters = characters });
                }
            });

            // this is where we will send the connecting client everything they need to play.
            // we will also update all players on the client's MapId that there is a new player.
            _netProcessor.SubscribeReusable<ClientRealm_TransferWorld, NetPeer>((transfer, peer) =>
            {
                Entity thisEntity = peer.Tag as Entity;
                WorldSessionComponent thisSession = thisEntity.GetComponent<WorldSessionComponent>();

                using (var ctx = new RealmContext())
                {
                    var activeCharacter = ctx.Characters
                        .Where(a => a.AccountId == thisSession.Account.Id)
                        .FirstOrDefault(c => c.CharacterId == transfer.LocalCharacterId);

                    thisSession.Character = activeCharacter;
                }
                thisSession.InitializeGameComponents();
                thisEntity.Name = thisSession.Character.Name;

                // let the client create their local player object.
                Send(peer, new RealmClient_CreateLocalPlayer()
                {
                    MapId = "world1",
                    ZoneX = thisSession.Character.XPosition,
                    ZoneY = thisSession.Character.YPosition
                });
                Console.WriteLine($"{thisEntity.Name} is entering the world!");

                SendToExcept(thisEntity.Name, new RealmClient_CreateNetPlayer() { Name = thisEntity.Name, ZoneX = thisSession.Character.XPosition, ZoneY = thisSession.Character.YPosition });

                var allSessionsExceptThis = Scene.FindComponentsOfType<WorldSessionComponent>().Where(session => session.Account.Id != thisSession.Account.Id).ToList();

                for (int i = 0; i < allSessionsExceptThis.Count; i++)
                {
                    var otherSession = allSessionsExceptThis[i];
                    SendTo(thisEntity.Name, new RealmClient_CreateNetPlayer() { Name = otherSession.Entity.Name, ZoneX = otherSession.Entity.Position.X, ZoneY = otherSession.Entity.Position.Y });
                }

                // tells the client they can enter the world.
                SendTo(thisEntity.Name, new RealmClient_EnterWorld());
            });

            _netProcessor.SubscribeReusable<ClientRealm_DeleteCharacter, NetPeer>((deletion, peer) =>
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
                            .Where(c => c.CharacterId == deletion.CharacterId)
                            .ExecuteDelete()) > 0;
                    }

                    if (isSuccess)
                    {
                        Console.WriteLine($"Account ID: {session.Account.Id} is deleting character id: {deletion.CharacterId}");
                        using (var ctx = new RealmContext())
                        {
                            List<RemoteCharacter> characters = new List<RemoteCharacter>();

                            // todo: get db charactes based on account id.

                            foreach (var character in ctx.Characters.Where(a => a.AccountId == session.Account.Id))
                                characters.Add(new RemoteCharacter(character.CharacterId, character.Name));

                            Console.WriteLine($"Sending {characters.Count} to client...");

                            SendSerializable(peer, new RealmClient_PlayerCharacters() { Characters = characters });
                        }
                        // todo: send character list to peer.
                    }
                }
            });

            _netEventListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader, peer);

            _netManager = new NetManager(_netEventListener);
            _netManager.Start(_port);

            // todo: implement prediction/reconciliation with packet loss and latency simulation.
            //_netManager.SimulatePacketLoss = true;
            //_netManager.SimulatePacketLoss = true;
            ////_netManager.SimulationPacketLossChance = 20;
            //_netManager.SimulationMinLatency = 100;
            //_netManager.SimulationMaxLatency = 350;

            _authListener = new EventBasedNetListener();
            _authListener.PeerConnectedEvent += (peer) =>
            {
                // todo: grab from config.
                SendToAuthserver(new RealmAuth_Registrar() { Name = "PTR", Ip = _hostName, Port = _port });
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

        public static void SendTo<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            var peer = _netManager.ConnectedPeerList.Where(p => (p.Tag as Entity).Name.Equals(gObjectId)).FirstOrDefault();

            if (peer != null)
                Send(peer, packet, delivery);
        }

        public static void SendToAll<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            for (int i = 0; i < _netManager.ConnectedPeersCount; i++)
                Send(_netManager.ConnectedPeerList[i], packet, delivery);
        }

        public static void SendToAuthserver<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(_authNetManager, packet, delivery);

        static void Main(string[] args)
            => new Program();
    }
}
