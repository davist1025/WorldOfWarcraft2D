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
    /// A singular copy of a given item in the database.
    /// </summary>
    public class ItemComponent : Component
    {
        public int Id { get; init; }
        public string Model_UI { get; init; }
        public string Model_Rendered { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public ItemType Descriptor { get; init; }
        public bool IsStackable { get; init; }

        public ItemComponent(Item itemData)
        {
            Id = itemData.Id;
            Model_UI = itemData.Model_UI;
            Model_Rendered = itemData.Model_Rendered;
            Name = itemData.Name;
            Description = itemData.Description;
            Descriptor = itemData.Descriptor;
            IsStackable = itemData.IsStackable;
        }

        public override void OnAddedToEntity()
        {
            // todo: add child components here maybe?
        }
    }
}
