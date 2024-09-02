using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Server.Shared.Serializable;
using static WoW.Server.Shared.Vocab;

namespace WoW.Server.Shared
{
    public class AuthRealm_SessionVerification : INetSerializable
    {
        public PlayerAccount User;

        public void Deserialize(NetDataReader reader)
        {
            User = new PlayerAccount
            {
                Id = reader.GetInt(),
                SessionId = reader.GetString(),
                Security = (SecurityLevel)reader.GetInt()
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(User.Id);
            writer.Put(User.SessionId);
            writer.Put((int)User.Security);
        }
    }
}
