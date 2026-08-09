using LiteNetLib;
using LiteNetLib.Utils;
using Nez;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components.GUI;
using WoW.Client.Components.Player;
using WoW.Framework.Logging;
using static WoW.Client.Global;
using static WoW.Framework.Network.NetworkManager;

namespace WoW.Client.Network
{
    /// <summary>
    /// The client implementation for networked events.
    /// </summary>
    internal class NetworkEventListener : INetEventListener
    {
        private Dictionary<byte, Action<NetDataReader>> _packetHandlers;

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            //Nez.Debug.Log($"Latency Update: {latency}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketOpCode opCode = (PacketOpCode)reader.GetByte();

            if (_packetHandlers.ContainsKey((byte)opCode))
                _packetHandlers[(byte)opCode]?.Invoke(reader);
            else
                Logger.Print($"No packet handler for OpCode '0x{opCode.ToString("X2")}' does not exist; packet will not be processed.", Framework.Utils.LogEntryType.Warning);
        }

        /// <summary>
        /// Processed when the client successfully connects to a server.
        /// </summary>
        /// <param name="peer"></param>
        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Print($"Connected to server.", Framework.Utils.LogEntryType.Debug);

            Global.Peer = peer;
            _packetHandlers = new Dictionary<byte, Action<NetDataReader>>
            {
                { (byte)PacketOpCode.SMSG_AUTH_LOGON, NetPacketManager.ReadLogonResponse },
                { (byte)PacketOpCode.SMSG_AUTH_REALMLIST, NetPacketManager.ReadRealmlist },
                { (byte)PacketOpCode.SMSG_REALM_CHARACTER_LIST, NetPacketManager.ReadCharacterList },
                { (byte)PacketOpCode.SMSG_REALM_ENTER_WORLD, NetPacketManager.ReadEnterWorld },
                { (byte)PacketOpCode.SMSG_REALM_CREATE_ACTOR,  NetPacketManager.ReadNewActor },
                { (byte)PacketOpCode.SMSG_REALM_MOVE, NetPacketManager.ReadMovementUpdate },
                { (byte)PacketOpCode.SMSG_REALM_DISCONNECT, NetPacketManager.ReadDisconnection }
            };

            switch (Global.PeerState)
            {
                case GameNetworkState.Auth_LoggingIn:
                    var imguiManager = Core.Scene.FindComponentOfType<ImGuiMainMenuManagerComponent>();
                    var login = imguiManager.GetLogin();

                    NetPacketManager.BuildLogon(login.Split(":"));
                    break;
                case GameNetworkState.Realm:
                    NetDataWriter writer = new NetDataWriter(true);
                    writer.Put((byte)PacketOpCode.CMSG_REALM_CONNECT);

                    NetFootprintComponent myFootprint = Global.Player.GetComponent<NetFootprintComponent>();
                    writer.Put(myFootprint.SessionId);
                    Global.Network.SendToServer(writer);
                    break;
            }
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
