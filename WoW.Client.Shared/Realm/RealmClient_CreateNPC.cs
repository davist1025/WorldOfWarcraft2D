using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;

namespace WoW.Client.Shared.Realm
{
    public class RealmClient_CreateNPC : INetSerializable
    {
        public RemoteNPC Data;

        public void Deserialize(NetDataReader reader)
        {
            Data = new RemoteNPC()
            {
                Name = reader.GetString(),
                Flags = reader.GetInt(),
                Level = reader.GetInt(),
                X = reader.GetFloat(),
                Y = reader.GetFloat(),
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Data.Name);
            writer.Put(Data.Flags);
            writer.Put(Data.Level);
            writer.Put(Data.X);
            writer.Put(Data.Y);
        }
    }
}
