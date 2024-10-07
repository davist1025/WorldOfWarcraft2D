using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Serializable;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// Tells the player to create a new GameObject that can have flags, which tell the player how to handle them.
    /// </summary>
    public class RealmClient_CreateNetObject : INetSerializable
    {
        public string Id;
        public SerializableCreature Creature;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Creature.Name);
            writer.Put(Creature.SubName);
            writer.Put(Creature.DisplayId);
            writer.Put(Creature.IsTargetable);
            writer.Put(Creature.IsAggressive);
            writer.Put((int)Creature.Flags);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetString();
            Creature = new SerializableCreature()
            {
                Name = reader.GetString(),
                SubName = reader.GetString(),
                DisplayId = reader.GetString(),
                IsTargetable = reader.GetBool(),
                IsAggressive = reader.GetBool(),
                Flags = (GameObjectFlags)reader.GetInt(),
            };
        }
    }
}
