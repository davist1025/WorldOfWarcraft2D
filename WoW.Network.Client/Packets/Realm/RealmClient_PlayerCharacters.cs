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
    public class RealmClient_PlayerCharacters : INetSerializable
    {
        public List<CharacterMetadataObject> Characters;

        public void Deserialize(NetDataReader reader)
        {
            Characters = new List<CharacterMetadataObject>();
            int characterCount = reader.GetInt();

            for (int i = 0; i < characterCount; i++)
            {
                int id = reader.GetInt();
                string name = reader.GetString();
                int raceId = reader.GetInt();
                int hairId = reader.GetInt();
                string mapId = reader.GetString();

                Characters.Add(new CharacterMetadataObject(id, name, (ActorRaceType)raceId, hairId, mapId));
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Characters.Count);

            for (int i = 0; i < Characters.Count; i++)
            {
                var character = Characters[i];

                writer.Put(character.Id);
                //writer.Put(character.Uid);
                writer.Put(character.Name);
                writer.Put((int)character.Race);
                writer.Put(character.Hair);
                writer.Put(character.MapId);
            }
        }
    }
}
