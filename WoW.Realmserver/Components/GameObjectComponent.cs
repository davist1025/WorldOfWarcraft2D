using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Realmserver.Components.Scripts;
using WoW.Realmserver.DB.Model;

namespace WoW.Realmserver.Components
{
    /// <summary>
    /// Defines a GameObject which can tick in the game world.
    /// 
    /// Can contain behavior (routines), listen for trigger events (i.e right-click), etc.
    /// Only operates on the given MapId.
    /// </summary>
    public class GameObjectComponent : Component
    {
        public string Id { get; set; }
        public string MapId { get; init; }
        public Creature Creature { get; init; }

        public ActionEventScript Script { get; set; }

        public GameObjectComponent(string mapId, Creature creature)
        {
            MapId = mapId;
            Creature = creature;
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
