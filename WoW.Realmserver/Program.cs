using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using Nez.Systems;
using Nez.Tiled;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using WoW.Database;
using WoW.Database.Models;
using WoW.Database.Models.Auth;
using WoW.Database.Models.Realm.Character;
using WoW.Database.Models.Realm.Items;
using WoW.Framework.Logging;
using WoW.Network;
using WoW.Network.Objects;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using WoW.Realmserver.Components;
using WoW.Realmserver.Content;
using WoW.Realmserver.Data;
using WoW.Realmserver.Network;
using static WoW.Framework.Utils;

namespace WoW.Realmserver
{
    internal class Program : CoreHeadless
    {
        public static NetworkController Network;
        public static WorldContentManager Content;

        public static float DeltaTime = 0f;
        public const float TickRate = 0.1f;

        public static Queue<PendingPlayer> PendingPlayers = new Queue<PendingPlayer>();
        public static List<Entity> ItemReferences = new List<Entity>();

        public Program()
        {
            Console.Title = "Realmserver";
            TiledMapLoader.IsHeadless = true;
            Scene.IsHeadless = true;

            IsFixedTimeStep = true;
            Scene = new WorldScene();

            Content = new WorldContentManager();

            Network = new NetworkController();
            Network.OnProcessorSubscribe += ProcessorSubscription;
            Network.OnClientDisconnect += ClientDisconnection;
            Network.StartServer(ConfigurationManager.AppSettings["hostname"], Convert.ToInt32(ConfigurationManager.AppSettings["port"]));

            // load content.
            Content.LoadTiled();

            // todo: ensurecreated for testing.
            // data doesn't need to persist across test runs, and can be initialized on startup.

            Logger.Print("Initializing database...", LogEntryType.Process);
            EnvironmentContext.AppSettings = ConfigurationManager.AppSettings;

            using (var ctx = new RealmContext())
            {
                Entity mapEntity = null;

                Logger.Print("Verifying racial spawn locations...", LogEntryType.Process);
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

                Logger.Print("Loading all items...", LogEntryType.Process);

                var items = ctx.Items.ToList();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var itemEntityRef = Scene.CreateEntity($"{item.Id}_{item.Name.ToLower()}");
                    var itemComponent = itemEntityRef.AddComponent(new ItemComponent(item));

                    ItemReferences.Add(itemEntityRef);
                }
                Logger.Print($"Loaded {ItemReferences.Count} items.", LogEntryType.Debug);

                ctx.SaveChanges();
            }

            while (true)
            {
                Network.Poll();
                Tick();
            }
        }

        private void ClientDisconnection(NetPeer peer, DisconnectInfo info)
        {
            PacketManager.OnClientDisconnected(peer, info);
        }

        private void ProcessorSubscription()
        {
            Network.Processor.RegisterNestedType<Vector2Serializable>();
            Network.Processor.SubscribeReusable<ClientRealm_Movement, NetPeer>((movement, peer) => PacketManager.OnPlayerMove(movement, peer));

            Network.Processor.SubscribeReusable<ClientRealm_TransferLogon, NetPeer>((transfer, peer) => PacketManager.OnPlayerTransferToRealm(transfer, peer));

            //Network.Processor.SubscribeNetSerializable<AuthRealm_SessionVerification, NetPeer>((session, peer) => PacketManager.OnAuthSessionVerification(session, peer));

            Network.Processor.SubscribeReusable<ClientRealm_CreateCharacter, NetPeer>((request, peer) => PacketManager.OnPlayerCreateCharacter(request, peer));

            // this is where we will send the connecting client everything they need to play.
            // we will also update all players on the client's MapId that there is a new player.
            Network.Processor.SubscribeReusable<ClientRealm_TransferWorld, NetPeer>((transfer, peer) => PacketManager.OnPlayerJoinWorld(transfer, peer));

            Network.Processor.SubscribeReusable<ClientRealm_DeleteCharacter, NetPeer>((deletion, peer) => PacketManager.OnPlayerDeleteCharacter(deletion, peer));

            // not entirely sure if this packet is necessary.
            Network.Processor.SubscribeReusable<ClientRealm_RequestCharacterList, NetPeer>((req, peer) => PacketManager.OnPlayerRequestCharacters(req, peer));

            Network.Processor.SubscribeReusable<ClientRealm_TabTarget, NetPeer>((req, peer) => PacketManager.OnTabTargetRequest(peer));

            Network.Processor.SubscribeReusable<ChatMessageObject, NetPeer>((newChat, peer) => PacketManager.OnPlayerChat(newChat, peer));
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
                        Account account = authCtx.Accounts.FirstOrDefault(a => a.SessionId.ToLower().Equals(sessionId));

                        if (account != null)
                        {
                            PlayerComponent newSession = new PlayerComponent(account);
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
            => Network.Processor.Send(peer, packet, delivery);

        /// <summary>
        /// Used to send an object which is not readily recognized by LNL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="peer"></param>
        /// <param name="packet"></param>
        /// <param name="delivery"></param>
        public static void SendSerializable<T>(NetPeer peer, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => Network.Processor.SendNetSerializable(peer, packet, delivery);

        public static void SendSerializable<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
            => SendSerializable(Network.GetAllPeers().Where(peer => (peer.Tag as Entity).Name.ToLower().Equals(gObjectId)).First(), packet);

        public static void SendToExcept<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            var peersExcept = Network.GetAllPeers().Where(p => !(p.Tag as Entity).Name.Equals(gObjectId)).ToArray();

            for (int i = 0; i < peersExcept.Length; i++)
                Send(peersExcept[i], packet, delivery);
        }

        public static void SendTo<T>(string gObjectId, T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            if (packet.GetType().IsAssignableTo(typeof(INetSerializable)))
                Logger.Print("Failed to send packet: Attempting to send a NetSerialized packet through a non-serializable channel; packet may arrive incomplete.", LogEntryType.Error);
            else
            {
                var peer = Network.GetAllPeers().Where(p => (p.Tag as Entity).Name.ToLower().Equals(gObjectId)).FirstOrDefault();

                if (peer != null)
                    Send(peer, packet, delivery);
            }
        }

        public static void SendToAll<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            var allPeers = Network.GetAllPeers();

            for (int i = 0; i < allPeers.Count; i++)
                Send(allPeers[i], packet, delivery);
        }

        static void Main(string[] args)
            => new Program();
    }
}
