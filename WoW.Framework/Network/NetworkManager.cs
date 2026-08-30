using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;

namespace WoW.Framework.Network
{
    public class NetworkManager
    {
        /// <summary>
        /// Different definitions for packet types, easily identifiable by byte.
        /// </summary>
        public enum PacketOpCode
        {
            CMSG_AUTH_LOGON = 1,
            SMSG_AUTH_LOGON = 2,

            #region Auth logon response codes
            SMSG_AUTH_LOGON_SUCCESS = 101, // argon2 verified.
            SMSG_AUTH_LOGON_PASSWORD = 102, // incorrect password.
            SMSG_AUTH_LOGON_MISMATCH = 103, // game/server version mismatch.
            #endregion

            SMSG_AUTH_REALMLIST = 3, // realmlist
            CMSG_REALM_CONNECT = 4,
            CMSG_REALM_CHARACTER_CREATE = 5,
            CMSG_REALM_CHARACTER_DELETE = 6,
            CMSG_REALM_CHARACTER_LIST = 7,
            CMSG_REALM_ENTER_WORLD = 8,
            CMSG_REALM_MOVE = 9,

            SMSG_REALM_CHARACTER_CREATE = 10, // parent packet to child response codes.

            #region Realm character creation response codes

            SMSG_REALM_CHARACTERCREATE_SUCCESS = 201, // simple success; packet #10 is sent directly after this to update client-side lists.
            SMSG_REALM_CHARACTERNAME_INUSE = 202, // name in-use.
            SMSG_REALM_CHARACTERNAME_RESTRICTED = 203, // name restricted.

            #endregion

            SMSG_REALM_CREATE_ACTOR = 11, // players (local and networked), npcs
            SMSG_REALM_DISCONNECT = 12, // kick
            SMSG_REALM_ENTER_WORLD = 13, // "enter world" confirmation with additional initial logon data
            SMSG_REALM_MOVE = 14, // notify all necessary players of another's movement.
            SMSG_REALM_MOVE_RECONCILE = 15, // movement correction.
            SMSG_REALM_CHARACTER_LIST = 16, // the player's character list within the main menu.

            SMSG_REALM_SESSION_TRANSFER_AUTH = 17, // confirm the session transfer with the authentication server.
            SMSG_AUTH_SESSION_TRANSFER_CONFIRMATION = 18,

            CMSG_REALM_CHAT = 19
        }

        private INetEventListener _listener;
        private static NetManager _netManager;

        public NetworkManager(INetEventListener listener)
        {
            _listener = listener;

            _netManager = new NetManager(listener)
            {
                UnconnectedMessagesEnabled = true
            };
        }

        public void Update()
        {
            _netManager.PollEvents();
        }

        public void StartClient()
            => _netManager.Start();

        /// <summary>
        /// Attempt a connection to the given host.
        /// </summary>
        /// <param name="hostname"></param>
        public void ConnectTo(string[] hostname)
        {
            string endpoint = hostname[0];
            int port = int.Parse(hostname[1]);

            _netManager.Connect(endpoint, port, "");
        }

        public void StartServer(string[] hostname)
        {
            string endpoint = hostname[0];
            int port = int.Parse(hostname[1]);

            _netManager.Start(endpoint, "", port);

            Logger.Print($"Listening on {port}.", Utils.LogEntryType.Network);
        }

        /// <summary>
        /// Disconnect ourselves.
        /// </summary>
        public void Disconnect() => _netManager.DisconnectAll();

        /// <summary>
        /// Enables latency simulation on the acting library. 
        /// 
        /// Can or cannot include packet loss.
        /// </summary>
        /// <param name="includePacketLoss"></param>
        /// <param name="minSimLatency"></param>
        /// <param name="maxSimLatency"></param>
        public void RunLatencySimulation(bool includePacketLoss = true, int packetLossChanceInPercent = 10, int minSimLatency = 150, int maxSimLatency = 400)
        {
            _netManager.SimulateLatency = true;
            _netManager.SimulatePacketLoss = includePacketLoss;
            if (includePacketLoss)
                _netManager.SimulationPacketLossChance = packetLossChanceInPercent;
            _netManager.SimulationMinLatency = minSimLatency;
            _netManager.SimulationMaxLatency = maxSimLatency;
        }

        /// <summary>
        /// Send the data of a <see cref="NetDataWriter"/> to the server.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="deliveryMethod"></param>
        public void SendToServer(NetDataWriter writer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            _netManager.FirstPeer.Send(writer, deliveryMethod);
            Logger.Print($"Sent OpCode: {(PacketOpCode)writer.Data[0]} w/ length: {writer.Data.Length} to the server.", Utils.LogEntryType.Debug);
        }

        /// <summary>
        /// Send data to one client directly.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="deliveryMethod"></param>
        public void SendToClient(NetPeer peer, NetDataWriter writer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            peer.Send(writer, deliveryMethod);
        }

        public void SendToClient(int peerId, NetDataWriter writer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            NetPeer peer = _netManager.Single(connection => connection.Id == peerId);

            if (peer != null)
                peer.Send(writer, deliveryMethod);
            else
                Logger.Print($"Unable to send packet of type '{(PacketOpCode)writer.Data[0]}' to peer: NetPeer with ID '{peerId}' does not exist!", Utils.LogEntryType.Fatal);
        }

        /// <summary>
        /// Sends data to all clients except for the one with the given unique network id.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="accountId"></param>
        /// <param name="delivery"></param>
        public void SendToAllExcept(NetDataWriter writer, int peerId = -1, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            if (peerId == -1)
            {
                Logger.Print($"Please provide a valid PeerId to exclude from this packet ({peerId}).", Utils.LogEntryType.Error);
                return;
            }

            IEnumerable<NetPeer> allPeersExcludingOne = _netManager.ConnectedPeerList.Where(peer => peer.Id != peerId);

            foreach (var peer in allPeersExcludingOne)
            {
                Logger.Print($"Sending '{(PacketOpCode)writer.Data[0]}' to peer w/ id: '{peer.Id}'.", Utils.LogEntryType.Network);

                peer.Send(writer, deliveryMethod);
            }
        }

        /// <summary>
        /// Send an unconnected message to the given end point.
        /// 
        /// Useful for Realmserver -> Authserver transmissions.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="endPoint"></param>
        public void SendUnconnected(NetDataWriter writer, IPEndPoint endPoint)
        {
            _netManager.SendUnconnectedMessage(writer, endPoint);
        }

        public NetPeer GetPeerWithTag(object tag) => _netManager.ConnectedPeerList.Find(peer => peer.Tag.Equals(tag));

        /// <summary>
        /// Returns the round-trip time to the server.
        /// </summary>
        /// <returns></returns>
        public int GetPing() => _netManager.FirstPeer.Ping;

        /// <summary>
        /// Checks if the NetManager object is polling, and whether the this NetManager is connected to an endpoint.
        /// 
        /// Useful in client-mode.
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() => 
            _netManager.IsRunning 
            && _netManager.FirstPeer != null
            && _netManager.FirstPeer.ConnectionState == ConnectionState.Connected;
    }
}

