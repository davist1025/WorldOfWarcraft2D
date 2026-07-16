using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network
{
    [Obsolete("Being removed as of 7/14.")]
    /// <summary>
    /// The home of the NetManager. Can be utilized by client and server(s).
    /// </summary>
    public class NetworkController
    {
        private EventBasedNetListener _listener;

        /// <summary>
        /// The packet processor.
        /// 
        /// Use this to subscribe new handlers to packet objects. 
        /// Example: NetworkController.Processor.SubscribeReusable<TObject, NetPeer>(PacketManager.HandleTObject);
        /// </summary>
        public NetPacketProcessor Processor;
        public Action OnProcessorSubscribe;
        public Action<NetPeer, DisconnectInfo> OnClientDisconnect;
        public Action<NetPeer> OnPeerConnect;

        private NetManager _manager; 

        public NetworkController()
        {
            _listener = new EventBasedNetListener();
            Processor = new NetPacketProcessor();
            _manager = new NetManager(_listener);

            _listener.ConnectionRequestEvent += (req) => req.Accept();
            _listener.PeerConnectedEvent += (peer) => OnPeerConnect?.Invoke(peer);
            _listener.PeerDisconnectedEvent += (peer, reason) => OnClientDisconnect?.Invoke(peer, reason);
            _listener.NetworkReceiveEvent += (peer, reader, method) => Processor.ReadAllPackets(reader, peer);
        }

        /// <summary>
        /// Call this in the client/server tick/update function.
        /// </summary>
        public void Poll()
            => _manager.PollEvents();

        /// <summary>
        /// Start the client network manager.
        /// </summary>
        public void StartClient()
        {
            OnProcessorSubscribe?.Invoke();
            _manager.Start();
        }

        /// <summary>
        /// Closes any connection if one exists and connects to the given server endpoint.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="port"></param>
        public void ConnectTo(string endPoint = "127.0.0.1", int port = 8080)
        {
            Disconnect();

            _manager.Connect(endPoint, port, "");
        }

        /// <summary>
        /// Start the server network manager on a host/ip to listen on for connections/packets.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public void StartServer(string hostname = "127.0.0.1", int port = 1111)
        {
            OnProcessorSubscribe?.Invoke();
            _manager.Start(hostname, "", port);
        }

        /// <summary>
        /// Sends a reusable object to the given peer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="peer"></param>
        /// <param name="obj"></param>
        /// <param name="deliveryMethod"></param>
        public void SendToPeer<T>(NetPeer peer, T obj, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            Processor.Send(peer, obj, deliveryMethod);
        }

        /// <summary>
        /// Sends a serializable object to the given peer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="peer"></param>
        /// <param name="obj"></param>
        /// <param name="deliveryMethod"></param>
        public void SendSerializableToPeer<T>(NetPeer peer, T obj, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable
        {
            Processor.SendNetSerializable(peer, obj, deliveryMethod);
        }

        /// <summary>
        /// Sends a reusable object o all peers.
        /// 
        /// This function can optionally accept a peer that should be exempt from this data notification.
        /// </summary>
        public void SendToAllPeers<T>(T obj, int exemptPeerId = -1, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            NetPeer[] sendList = new NetPeer[(exemptPeerId > -1) ? _manager.ConnectedPeersCount - 1 :  _manager.ConnectedPeersCount];

            if (exemptPeerId > -1)
            {
                var allPeersExcept = _manager.Where(peer => peer.Id != exemptPeerId).ToArray();
                for (int i = 0; i < allPeersExcept.Length; i++)
                    sendList[i] = allPeersExcept[i];
            }
            else
                _manager.ConnectedPeerList.CopyTo(sendList);

            for (int i = 0; i < sendList.Length; i++)
                Processor.Send(sendList[i], obj, deliveryMethod);
        }

        /// <summary>
        /// Sends a serializable object to the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="deliveryMethod"></param>
        public void SendToServer<T>(T obj, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            Processor.Send(_manager.FirstPeer, obj, deliveryMethod);
        }

        /// <summary>
        /// Verifies that the network manager is runinng/connected.
        /// 
        /// Client only.
        /// </summary>
        /// <returns></returns>
        public bool IsRunning() =>
            _manager.IsRunning
            && _manager.FirstPeer != null
            && _manager.FirstPeer.ConnectionState == ConnectionState.Connected;

        /// <summary>
        /// Returns a list of all connected peers.
        /// 
        /// Client-mode will return one.
        /// </summary>
        /// <returns></returns>
        public List<NetPeer> GetAllPeers() => _manager.ConnectedPeerList;

        /// <summary>
        /// Closes out all Network connections.
        /// </summary>
        public void Disconnect()
        {
            _manager.DisconnectAll();
        }
    }
}
