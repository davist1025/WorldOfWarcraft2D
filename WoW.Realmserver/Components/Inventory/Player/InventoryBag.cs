using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models.Realm.Items;

namespace WoW.Realmserver.Components.Inventory.Player
{
    public class InventoryBag
    {
        public int SlotIndex { get; private set; }
        public int BagItemId { get; private set; }

        private List<BagItem> _items = new List<BagItem>();

        public InventoryBag(int slotIndex, int bagItemId)
        {
            SlotIndex = slotIndex;
            BagItemId = bagItemId;
        }

        public void LoadItems(List<CharacterBagInventoryIndex> items)
        {
            foreach (var item in items)
            {
                _items.Add(new BagItem(item.BagSpaceIndex, item.ItemId, item.ItemStackCount));
            }
        }
    }
}
