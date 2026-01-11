using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;

namespace WoW.Network.Client.Packets.Realm
{
    public class RealmClient_CreateNPC : INetSerializable
    {
        public NpcMetadata Metadata;

        public void Deserialize(NetDataReader reader)
        {
            Metadata = new NpcMetadata()
            {
                WorldId = reader.GetString(),
                Name = reader.GetString(),
                ModelId = reader.GetString(),
                Flags = (NpcTypeFlags)reader.GetInt(),
                Level = reader.GetInt(),
                MapId = reader.GetString(),
                X = reader.GetFloat(),
                Y = reader.GetFloat(),
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Metadata.WorldId);
            writer.Put(Metadata.Name);
            writer.Put(Metadata.ModelId);
            writer.Put((int)Metadata.Flags);
            writer.Put(Metadata.Level);
            writer.Put(Metadata.MapId);
            writer.Put(Metadata.X);
            writer.Put(Metadata.Y);
        }
    }
}
