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
            Logger.Print($"{request.RemoteEndPoint} is trying to connect...", Utils.LogEntryType.Debug);
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
            PacketOpCode packetId = (PacketOpCode)reader.GetByte();

            // todo: check for validity against realmlist endpoints to ensure security. for now, we dont care :p
            switch (packetId)
            {
                case PacketOpCode.SMSG_REALM_DISCONNECT: NetPacketManager.ReadClientDisconnection(reader); break;
            }
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
