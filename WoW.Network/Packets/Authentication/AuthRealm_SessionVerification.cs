using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Network.Objects;
using static WoW.Framework.Utils;

namespace WoW.Network.Packets.Authentication
{
    public class AuthRealm_SessionVerification : INetSerializable
    {
        public AccountMetadataObject User;

        public void Deserialize(NetDataReader reader)
        {
            User = new AccountMetadataObject(reader.GetInt(), reader.GetString(), (AccountSecurityType)reader.GetInt());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(User.Id);
            writer.Put(User.SessionId);
            writer.Put((int)User.SecurityLevel);
        }
    }
}
