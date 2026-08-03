using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
            CMSG_AUTH_LOGON = 0x00,
            CMSG_REALM_CONNECT = 0x04,
            CMSG_REALM_CHARACTER_CREATE = 0x05,
            CMSG_REALM_CHARACTER_DELETE = 0x06,
            CMSG_REALM_CHARACTER_LIST = 0x07,
            CMSG_REALM_ENTER_WORLD = 0x08,
            CMSG_REALM_MOVE = 0x09,

            SMSG_AUTH_LOGON = 0x01,

            #region Auth logon response codes
            SMSG_AUTH_LOGON_SUCCESS = 0x00, // argon2 verified.
            SMSG_AUTH_LOGON_PASSWORD = 0x01, // incorrect password.
            SMSG_AUTH_LOGON_MISMATCH = 0x02, // game/server version mismatch.
            #endregion

            SMSG_AUTH_REALMLIST = 0x02, // realmlist
            // todo: send an unconnected message for realm transfer protocol? not really sure if it's 100% necessary for the realm and auth to be in contact.

            SMSG_REALM_CHARACTER_CREATE = 0x0A, // parent packet to child response codes.

            #region Realm character creation response codes

            SMSG_REALM_CHARACTERCREATE_SUCCESS = 0x00, // simple success; 0x10 is sent directly after this to update client-side lists.
            SMSG_REALM_CHARACTERNAME_INUSE = 0x01, // name in-use.
            SMSG_REALM_CHARACTERNAME_RESTRICTED = 0x02, // name restricted.

            #endregion

            SMSG_REALM_CREATE_ACTOR = 0x0B, // players (local and networked), npcs
            SMSG_REALM_DISCONNECT = 0x0C, // kick
            SMSG_REALM_ENTER_WORLD = 0x0D, // "enter world" confirmation with additional initial logon data
            SMSG_REALM_MOVE = 0x0E, // notify all necessary players of another's movement.
            SMSG_REALM_MOVE_RECONCILE = 0x0F, // movement correction.
            SMSG_REALM_CHARACTER_LIST = 0x10, // the player's character list within the main menu.
        }

        private INetEventListener _listener;
        private static NetManager _netManager;

        public NetworkManager(INetEventListener listener)
        {
            _listener = listener;

            _netManager = new NetManager(listener);
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
                peer.Send(writer, deliveryMethod);
        }
    }
}
