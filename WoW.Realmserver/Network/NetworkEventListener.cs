using LiteNetLib;
using LiteNetLib.Utils;
using Nez;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Framework;
using WoW.Framework.Logging;
using WoW.Realmserver.Components;
using static WoW.Framework.Network.NetworkManager;
using static WoW.Framework.Utils;

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
            Logger.Print($"{request.RemoteEndPoint} is trying to connect to the realmserver...", LogEntryType.Debug);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketOpCode packetId = (PacketOpCode)reader.GetByte();

            switch (packetId)
            {
                case PacketOpCode.CMSG_REALM_CONNECT: NetPacketManager.ReadSessionTransfer(peer, reader); break;
                case PacketOpCode.CMSG_REALM_ENTER_WORLD: NetPacketManager.ReadEnterWorld(peer, reader); break;
                case PacketOpCode.CMSG_REALM_MOVE: NetPacketManager.ReadMovementUpdate(peer, reader, deliveryMethod); break;
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            PacketOpCode packetId = (PacketOpCode)reader.GetByte();

            switch (packetId)
            {
                case PacketOpCode.SMSG_AUTH_SESSION_TRANSFER_CONFIRMATION: NetPacketManager.ReadSessionTransferConfirmation(reader); break;
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var entity = peer.Tag as Entity;
            var session = entity.GetComponent<SessionComponent>();

            Logger.Print($"Player '{session.GetSelectedCharacter().Name}' has left the world!", LogEntryType.Network);

            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.SMSG_REALM_DISCONNECT);
            writer.Put(session.SessionId);

            // tell the authserver this player has quit.
            Global.Network.SendUnconnected(writer, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8070));

            // tell all players this player has quit.
            NetDataWriter disconnectWriter = new NetDataWriter(true);
            disconnectWriter.Put((byte)PacketOpCode.SMSG_REALM_DISCONNECT);
            disconnectWriter.Put(session.NetworkId);
            Global.Network.SendToAllExcept(disconnectWriter, peer.Id);

            // update the character in the db.
            using (var ctx = new RealmContext())
            {
                ctx.Characters.Update(session.GetSelectedCharacter());
                ctx.SaveChanges();

                Logger.Print($"'{session.GetSelectedCharacter().Name}' has been saved to the database.", LogEntryType.Debug);
            }
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
