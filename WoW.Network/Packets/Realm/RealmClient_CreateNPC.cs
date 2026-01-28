using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Network.Objects;
using static WoW.Framework.Utils;

namespace WoW.Network.Packets.Realm
{
    public class RealmClient_CreateNPC : INetSerializable
    {
        public NpcMetadataObject Metadata;

        public void Deserialize(NetDataReader reader)
        {
            Metadata = new NpcMetadataObject()
            {
                Uid = reader.GetString(),
                Name = reader.GetString(),
                ModelId = reader.GetString(),
                Flags = (ActorFlagTypes)reader.GetInt(),
                Level = reader.GetInt(),
                MapId = reader.GetString(),
                Position = new Framework.Vector2S
                {
                    X = reader.GetFloat(),
                    Y = reader.GetFloat(),
                }
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Metadata.Uid);
            writer.Put(Metadata.Name);
            writer.Put(Metadata.ModelId);
            writer.Put((int)Metadata.Flags);
            writer.Put(Metadata.Level);
            writer.Put(Metadata.MapId);
            writer.Put(Metadata.Position.X);
            writer.Put(Metadata.Position.Y);
        }
    }
}
