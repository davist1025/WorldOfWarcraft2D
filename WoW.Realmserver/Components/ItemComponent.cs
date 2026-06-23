using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models.Realm.Items;

namespace WoW.Realmserver.Components
{
    /// <summary>
    /// Describes the base item information.
    /// </summary>
    public class ItemComponent : Component
    {
        public int Id;
        public string UIModel;
        public string GameModel;
        public string Name;
        public string Description;
        public ItemType Descriptor;

        public ItemComponent(Item itemEntry)
        {
            Id = itemEntry.Id;
            UIModel = itemEntry.Inventory_ModelId;
            GameModel = itemEntry.Rendered_ModelId;
            Name = itemEntry.Name;
            Description = itemEntry.Description;
            Descriptor = itemEntry.Descriptor;
        }
    }
}
