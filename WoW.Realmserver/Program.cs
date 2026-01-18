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
using WoW.Database.Models;
using WoW.Database.Models.Auth;
using WoW.Database.Models.Realm.Character;
using WoW.Framework.Logging;
using WoW.Network.Objects;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using WoW.Realmserver.Components;
using WoW.Realmserver.Content;
using static WoW.Framework.Utils;

namespace WoW.Realmserver
{
    internal class Program : CoreHeadless
    {
        private static NetManager _netManager;
        private EventBasedNetListener _netEventListener;
        private static NetPacketProcessor _netProcessor;

        public static RealmConfiguration Configuration;
        public static WorldContentManager Content;

        public static float DeltaTime = 0f;
        public const float TickRate = 0.1f;

        public static Queue<PendingPlayer> PendingPlayers = new Queue<PendingPlayer>();

        public Program()
        {
            Console.Title = "Realmserver";
            TiledMapLoader.IsHeadless = true;
            Scene.IsHeadless = true;

            IsFixedTimeStep = true;
            Scene = new WorldScene();

            Configuration = RealmConfiguration.Load();
            Content = new WorldContentManager();
            _netProcessor = new NetPacketProcessor();

            // load content.
            Content.LoadTiled();

            // todo: ensurecreated for testing.
            // data doesn't need to persist across test runs, and can be initialized on startup.

            //Console.WriteLine("Checking for script/NPC flag mismatch...");
            //using (var ctx = new RealmContext())
            //{
            //    // todo: check for multiple flags which should use scripts (i.e: dialogue, merchant, etc)
            //    var flagsNeedingScript = ctx.NPCs.Where(npc => ((NpcTypeFlags)npc.FlagType).HasFlag(NpcTypeFlags.CanDialogue)).ToList();
            //    foreach (var npc in flagsNeedingScript)
            //    {
            //        bool isMissingScript = ctx.Behaviors.Any(b => b.NpcId == npc.Id && b.Script == null);

            //        if (isMissingScript)
            //            Console.WriteLine($"NPC: {npc.Name} has dialogue, but there is no script attached!");
            //    }
            //}

            Logger.Print("Verifying default racial spawn locations...", LogEntryType.Process);
            using (var ctx = new RealmContext())
            {
                Entity mapEntity = null;

                if (!ctx.RaceSpawns.Any(spawn => spawn.RaceId == (int)ActorRaceType.Human))
                {
                    mapEntity = Scene.FindEntity("elwynn_forest");
                    var spawnerComponent = mapEntity.GetComponents<SpawnerComponent>().Where(spawner => spawner.IsPlayerSpawner).Single();

                    ctx.RaceSpawns.Add(new CharacterRaceSpawn()
                    {
                        RaceId = (int)ActorRaceType.Human,
                        MapId = "elwynn_forest",
                        X = spawnerComponent.Position.X,
                        Y = spawnerComponent.Position.Y,
                    });
                }

                if (!ctx.RaceSpawns.Any(spawn => spawn.RaceId == (int)ActorRaceType.Orc))
                {
                    ctx.RaceSpawns.Add(new CharacterRaceSpawn()
                    {
                        RaceId = (int)ActorRaceType.Orc,
                        MapId = "valley_of_trials",
                        X = 50f,
                        Y = 50f,
                    });
                }

                ctx.SaveChanges();
            }

            _netEventListener = new EventBasedNetListener();
            _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.PeerConnectedEvent += (peer) => { };
            _netEventListener.PeerDisconnectedEvent += (peer, reason) => PacketManager.OnClientDisconnected(peer, reason);

            _netProcessor.RegisterNestedType<Vector2Serializable>();
            _netProcessor.SubscribeReusable<ClientRealm_Movement, NetPeer>((movement, peer) => PacketManager.OnPlayerMove(movement, peer));
            
            _netProcessor.SubscribeReusable<ClientRealm_TransferLogon, NetPeer>((transfer, peer) => PacketManager.OnPlayerTransferToRealm(transfer, peer));

            //_netProcessor.SubscribeNetSerializable<AuthRealm_SessionVerification, NetPeer>((session, peer) => PacketManager.OnAuthSessionVerification(session, peer));

            _netProcessor.SubscribeReusable<ClientRealm_CreateCharacter, NetPeer>((request, peer) => PacketManager.OnPlayerCreateCharacter(request, peer));

            // this is where we will send the connecting client everything they need to play.
            // we will also update all players on the client's MapId that there is a new player.
            _netProcessor.SubscribeReusable<ClientRealm_TransferWorld, NetPeer>((transfer, peer) => PacketManager.OnPlayerJoinWorld(transfer, peer));

            _netProcessor.SubscribeReusable<ClientRealm_DeleteCharacter, NetPeer>((deletion, peer) => PacketManager.OnPlayerDeleteCharacter(deletion, peer));

            // not entirely sure if this packet is necessary.
            _netProcessor.SubscribeReusable<ClientRealm_RequestCharacterList, NetPeer>((req, peer) => PacketManager.OnPlayerRequestCharacters(req, peer));

            _netProcessor.SubscribeReusable<ClientRealm_TabTarget, NetPeer>((req, peer) => PacketManager.OnTabTargetRequest(peer));

            _netProcessor.SubscribeReusable<ChatMessageObject, NetPeer>((newChat, peer) => PacketManager.OnPlayerChat(newChat, peer));

            _netEventListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader, peer);

            _netManager = new NetManager(_netEventListener);
            _netManager.Start("127.0.0.1", "", Configuration.Port);

            /**
             * Some simulated packet/latency loss has been tested against the current netcode.
             * The important note is that all clients are currently synced well enough.
             * 
             * 12/2/25: Test/implement analytics when the server is running on a dedi.
             * 
             */ 

            while (true)
            {
                _netManager.PollEvents();
                Tick();
            }
        }

        public override void Update(float deltaTime)
        {
            DeltaTime = deltaTime;
            Scene.Update();

            using (var authCtx = new AuthContext())
            {
                if (PendingPlayers.TryDequeue(out var newPendingConnection))
                {
                    var sessionId = newPendingConnection.SessionId;

                    if (authCtx.Accounts.Any(x => x.SessionId == sessionId)) ;
                    {
                        // todo: can this object just be stored and tracked to avoid making PlayerAccount?
                        Account account = authCtx.Accounts.FirstOrDefault(a => a.SessionId.ToLower().Equals(sessionId));

                        if (account != null)
                        {
                            WorldSessionComponent newSession = new WorldSessionComponent(account);
                            Entity newEntity = Scene.CreateEntity(Guid.NewGuid().ToString());
                            newEntity.Tag = (int)ActorType.Networked;
                            newEntity.AddComponent(newSession);
                            newPendingConnection.Connection.Tag = newEntity;

                            Logger.Print($"Pending connection w/ Account ({account.Username}) has been verified.", LogEntryType.Network);

                            PacketManager.SendCharactersTo(newSession.Account.Id, newPendingConnection.Connection);
                        }
                    }
                }
            }
        }

        public static void Send<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(peer, packet, delivery);

        /// <summary>
        /// Used to send an object which is not readily recognized by LNL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="peer"></param>
        /// <param name="packet"></param>
        /// <param name="delivery"></param>
        public static void SendSerializable<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => _netProcessor.SendNetSerializable(peer, packet, delivery);

        public static void SendSerializable<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => SendSerializable(_netManager.ConnectedPeerList.Where(peer => (peer.Tag as Entity).Name.ToLower().Equals(gObjectId)).First(), packet);

        public static void SendSerializableToAll<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => _netProcessor.SendNetSerializable(_netManager, packet, delivery);


        public static void SendToExcept<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            var peersExcept = _netManager.ConnectedPeerList.Where(p => !(p.Tag as Entity).Name.Equals(gObjectId)).ToArray();

            for (int i = 0; i < peersExcept.Length; i++)
                Send(peersExcept[i], packet, delivery);
        }

        public static void SendTo<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            if (packet.GetType().IsAssignableTo(typeof(INetSerializable)))
                Logger.Print("Failed to send packet: Attempting to send a NetSerialized packet through a non-serializable channel; packet may arrive incomplete.", LogEntryType.Error);
            else
            {
                var peer = _netManager.ConnectedPeerList.Where(p => (p.Tag as Entity).Name.ToLower().Equals(gObjectId)).FirstOrDefault();

                if (peer != null)
                    Send(peer, packet, delivery);
            }
        }

        public static void SendToAll<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            for (int i = 0; i < _netManager.ConnectedPeersCount; i++)
                Send(_netManager.ConnectedPeerList[i], packet, delivery);
        }

        public static void SendToMapFromPlayer<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {


            var peer = _netManager.ConnectedPeerList.Where(p => (p.Tag as Entity).Name.ToLower().Equals(gObjectId)).FirstOrDefault();
            // todo: crash here when a player moves in the world while x # of players are on the char. select screen.
            // its suspected the if players are on the char. select screen while another player is in-game moving, it will crash the server.

            var entity = peer.Tag as Entity;
            var processor = Scene.FindComponentsOfType<TiledMapProcessor>().Where(processor => processor.Creatures.Contains(entity)).FirstOrDefault();
            var players = processor.Creatures.Where(e => e.HasComponent<WorldSessionComponent>() && !e.Name.ToLower().Equals(gObjectId)).ToArray();

            foreach (var p in players)
                SendTo(p.Name, packet, delivery);
        }

        static void Main(string[] args)
            => new Program();
    }
}
