using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;

namespace WoW.Client.Shared.Realm
{
    public class RealmClient_PlayerCharacters : INetSerializable
    {
        public List<RemoteCharacter> Characters;

        public void Deserialize(NetDataReader reader)
        {
            Characters = new List<RemoteCharacter>();
            int characterCount = reader.GetInt();

            for (int i = 0; i < characterCount; i++)
            {
                int id = reader.GetInt();
                string name = reader.GetString();

                Characters.Add(new RemoteCharacter(id, name));
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Characters.Count);

            for (int i = 0; i < Characters.Count; i++)
            {
                var character = Characters[i];

                writer.Put(character.CharacterId);
                writer.Put(character.CharacterName);
            }
        }
    }
}
