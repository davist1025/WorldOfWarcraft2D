using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Serializable;

namespace WoW.Client.Shared.Realm
{
    public class RealmClient_Character : INetSerializable
    {
        public SerializableCharacter Character;

        public void Deserialize(NetDataReader reader)
        {
            Character = new
                (reader.GetInt(),
                reader.GetInt(),
                reader.GetInt(),
                reader.GetString(),
                reader.GetInt(),
                (CharacterClassType)reader.GetInt());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Character.CharacterId);
            writer.Put(Character.GuildId);
            writer.Put(Character.Name);
            writer.Put(Character.Level);
            writer.Put((int)Character.Class);
        }
    }
}
