using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Serializable;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client
    /// Sent to all clients and describes a new character login.
    /// </summary>
    public class RealmClient_Connect : INetSerializable
    {
        public string Id;
        public SerializableCharacter PlayerCharacter;

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetString();
            PlayerCharacter = new
                (reader.GetInt(),
                reader.GetInt(),
                reader.GetInt(),
                reader.GetString(), 
                reader.GetInt(), 
                (CharacterClassType)reader.GetInt());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(PlayerCharacter.CharacterId);
            writer.Put(PlayerCharacter.RaceId);
            writer.Put(PlayerCharacter.GuildId);
            writer.Put(PlayerCharacter.Name);
            writer.Put(PlayerCharacter.Level);
            writer.Put((int)PlayerCharacter.Class);
        }
    }
}
