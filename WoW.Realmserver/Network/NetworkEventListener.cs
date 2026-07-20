using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework;
using WoW.Framework.Logging;
using static WoW.Framework.Network.NetworkManager;

namespace WoW.Realmserver.Network
{
    /// <summary>
    /// The realmserver handler for network events.
    /// </summary>
    internal class NetworkEventListener : INetEventListener
    {
        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept(); // arbitrarily accept connections for now.
            Logger.Print($"{request.RemoteEndPoint} is trying to connect to the realmserver...", Utils.LogEntryType.Debug);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketOpCode packetId = (PacketOpCode)reader.GetByte();

            switch (packetId)
            {
                case PacketOpCode.CMSG_REALM_CONNECT: NetPacketManager.ReadSessionTransfer(peer, reader); break;
                case PacketOpCode.CMSG_REALM_ENTER_WORLD: NetPacketManager.ReadEnterWorld(peer, reader); break;
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnPeerConnected(NetPeer peer)
        {
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        #region Unused functions
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }
        #endregion
    }
}
