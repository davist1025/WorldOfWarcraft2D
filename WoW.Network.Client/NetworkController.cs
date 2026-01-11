using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Network
{
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

        private NetManager _manager;

        public NetworkController()
        {
            _listener = new EventBasedNetListener();
            Processor = new NetPacketProcessor();
            _manager = new NetManager(_listener);

            _listener.ConnectionRequestEvent += (req) => req.Accept();
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
            => _manager.Start();

        /// <summary>
        /// Start the server network manager on a host/ip to listen on for connections/packets.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public void StartServer(string hostname = "127.0.0.1", int port = 1111)
            => _manager.Start(hostname, "", port);
    }
}
