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
    /// Realm -> Client.
    /// 
    /// Contains the player's session id as well as their list of characters on this realm.
    /// </summary>
    public class RealmClient_CharacterList : INetSerializable
    {
        public List<SerializableCharacter> Characters;

        public void Deserialize(NetDataReader reader)
        {
            Characters = new List<SerializableCharacter>();

            int characterCount = reader.GetInt();

            for (int i = 0; i < characterCount; i++)
            {
                int localId = reader.GetInt();
                int guildId = reader.GetInt();
                string name = reader.GetString();
                int level = reader.GetInt();
                CharacterClassType @class = (CharacterClassType)reader.GetInt();
                Characters.Add(new SerializableCharacter(localId, guildId, name, level, @class));
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Characters.Count);
            for (int i = 0; i < Characters.Count; i++)
            {
                SerializableCharacter character = Characters[i];
                writer.Put(character.CharacterId);
                writer.Put(character.GuildId);
                writer.Put(character.Name);
                writer.Put(character.Level);
                writer.Put((int)character.Class);
            }
        }
    }
}
