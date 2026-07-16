using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;
using WoW.Network;

namespace WoW.Authserver
{
    /// <summary>
    /// The authserver handler for network events.
    /// </summary>
    internal class NetworkEventListener : INetEventListener
    {
        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept(); // arbitrarily accept connections for now.
            Logger.Print($"{request.RemoteEndPoint} is trying to connect...", Framework.Utils.LogEntryType.Debug);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketOpCode packetId = (PacketOpCode)reader.GetByte();

            switch (packetId)
            {
                case PacketOpCode.CMSG_AUTH_LOGON: NetPacketManager.ReadLogin(peer, reader, deliveryMethod); break;
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Print($"{peer.EndPoint} has connected!", Framework.Utils.LogEntryType.Network);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        #region Unusued functions
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        #endregion
    }
}
