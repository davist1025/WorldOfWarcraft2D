using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client.
    /// Tells the client to create a new entity.
    /// 
    /// This packet is the only one to signify that the client should create a new Scene entity.
    /// </summary>
    public class RealmClient_CreateGameObject
    {
        public GameObjectType EntityType { get; set; }

        /// <summary>
        /// The unique id of the entity.
        /// 
        /// If player, this will be their session id.
        /// If NPC or otherwise, this will be their generated id.
        /// </summary>
        public string Id { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
    }
}
