using LiteNetLib.Utils;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;
using WoW.Network;

namespace WoW.Client.Network
{
    /// <summary>
    /// Handles incoming packet data.
    /// </summary>
    public static class NetPacketManager
    {
        /*
         * When writing packet data, the packet's OpCode always comes first to let the peer know what type of data is coming through.
         */ 

        /// <summary>
        /// Builds and sends the logon packet to the Authserver.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void BuildLogon(string[] loginData)
        {
            string username = loginData[0];
            string password = WoW.Framework.Utils.ToSha256(loginData[1]);

            NetDataWriter writer = new NetDataWriter(true);
            writer.Put((byte)PacketOpCode.CMSG_AUTH_LOGON);
            writer.Put(username);
            writer.Put(password);

            // other details?
            /*
             * ex: game version, OS, etc?
             */ 

            Global.Network.SendToServer(writer);
        }
    }
}
