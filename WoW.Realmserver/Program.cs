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

        public static RealmConfiguration Configuration;
        public static WorldContentManager Content;

        public static float DeltaTime = 0f;

        public static Dictionary<string, NetPeer> TransferSessions = new Dictionary<string, NetPeer>();

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

            Console.WriteLine("Checking for script/NPC flag mismatch...");
            using (var ctx = new RealmContext())
            {
                // todo: check for multiple flags which should use scripts (i.e: dialogue, merchant, etc)
                var flagsNeedingScript = ctx.NPCs.Where(npc => ((NpcTypeFlags)npc.FlagType).HasFlag(NpcTypeFlags.CanDialogue)).ToList();
                foreach (var npc in flagsNeedingScript)
                {
                    bool isMissingScript = ctx.Behaviors.Any(b => b.NpcId == npc.Id && b.Script == null);

                    if (isMissingScript)
                        Console.WriteLine($"NPC: {npc.Name} has dialogue, but there is no script attached!");
                }
            }

            _netEventListener = new EventBasedNetListener();
            _netEventListener.ConnectionRequestEvent += (req) => req.Accept();
            _netEventListener.PeerConnectedEvent += (peer) => { };
            _netEventListener.PeerDisconnectedEvent += (peer, reason) => PacketManager.OnClientDisconnected(peer, reason);

            _netProcessor.SubscribeReusable<ClientRealm_Movement, NetPeer>((movement, peer) => PacketManager.OnPlayerMove(movement, peer));
            
            _netProcessor.SubscribeReusable<ClientRealm_Chat, NetPeer>((chat, peer) => PacketManager.OnPlayerChat(chat, peer));

            _netProcessor.SubscribeReusable<ClientRealm_TransferLogon, NetPeer>((transfer, peer) => PacketManager.OnPlayerTransferToRealm(transfer, peer));

            _netProcessor.SubscribeNetSerializable<AuthRealm_SessionVerification, NetPeer>((session, peer) => PacketManager.OnAuthSessionVerification(session, peer));

            _netProcessor.SubscribeReusable<ClientRealm_CreateCharacter, NetPeer>((request, peer) => PacketManager.OnPlayerCreateCharacter(request, peer));

            // this is where we will send the connecting client everything they need to play.
            // we will also update all players on the client's MapId that there is a new player.
            _netProcessor.SubscribeReusable<ClientRealm_TransferWorld, NetPeer>((transfer, peer) => PacketManager.OnPlayerJoinWorld(transfer, peer));

            _netProcessor.SubscribeReusable<ClientRealm_DeleteCharacter, NetPeer>((deletion, peer) => PacketManager.OnPlayerDeleteCharacter(deletion, peer));

            // not entirely sure if this packet is necessary.
            _netProcessor.SubscribeReusable<ClientRealm_RequestCharacterList, NetPeer>((req, peer) => PacketManager.OnPlayerRwquestCharacters(req, peer));

            _netEventListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader, peer);

            _netManager = new NetManager(_netEventListener);
            _netManager.Start(Configuration.Port);

            // todo: implement prediction/reconciliation with packet loss and latency simulation.
            //_netManager.SimulatePacketLoss = true;
            //_netManager.SimulatePacketLoss = true;
            ////_netManager.SimulationPacketLossChance = 20;
            //_netManager.SimulationMinLatency = 100;
            //_netManager.SimulationMaxLatency = 350;

            _authListener = new EventBasedNetListener();
            _authListener.PeerConnectedEvent += (peer) =>
            {
                SendToAuthserver(new RealmAuth_Registrar() { Ip = Configuration.IpAddress, Port = Configuration.Port });
            };

            _authListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader, peer);
            _authNetManager = new NetManager(_authListener);
            _authNetManager.Start();
            _authNetManager.Connect("127.0.0.1", 8070, "");

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
            // todo: log stuff like this to file/console output.
            //if (packet.GetType().IsAssignableTo(typeof(INetSerializable)))
            //    Console.WriteLine("Attempting to send a NetSerialized packet through a non-serializable channel; packet may arrive incomplete.");

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
