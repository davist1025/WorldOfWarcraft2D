using LiteNetLib;
using Nez;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components.GUI;
using WoW.Framework.Logging;

namespace WoW.Client.Network
{
    /// <summary>
    /// The client implementation for networked events.
    /// </summary>
    internal class NetworkEventListener : INetEventListener
    {
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            Nez.Debug.Log($"Latency Update: {latency}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            // todo: process incoming packets.
        }

        /// <summary>
        /// Processed when the client successfully connects to a server.
        /// </summary>
        /// <param name="peer"></param>
        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Print($"Connected to server.", Framework.Utils.LogEntryType.Debug);

            Global.Peer = peer;

            switch (Global.PeerState)
            {
                case GameNetworkState.Auth_LoggingIn:
                    var imguiManager = Core.Scene.FindComponentOfType<ImGuiMainMenuManagerComponent>();
                    var login = imguiManager.GetLogin();

                    NetPacketManager.BuildLogon(login.Split(":"));
                    break;
            }
            // todo: send packet depending on state.
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        #region Unused functions
        public void OnConnectionRequest(ConnectionRequest request)
        {
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }
        #endregion
    }
}
