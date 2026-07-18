using Nez;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Database.Models.Realm.Items;

namespace WoW.Realmserver.Components.Inventory.Player
{
    /// <summary>
    /// Contains the inventory, including bag and equipment data, for each player that has any.
    /// </summary>
    public class InventoryComponent : Component
    {
        private List<InventoryBag> _bags; 

        public override void OnAddedToEntity()
        {
            List<CharacterBagIndex> characterBags;

            using (var ctx = new RealmContext())
            {
                // Get all bags for this character.
                characterBags = ctx.CharacterBags.Where(c => c.CharacterId == Entity.GetComponent<SessionComponent>().Character.CharacterId).ToList();

                foreach (var row in characterBags)
                {
                    var inventoryBag = new InventoryBag(row.BagSlotIndex, row.BagItemId);
                    // Get all items contained within this bag.
                    var thisBagsItems = ctx.CharacterBagInventories
                        .Where(inventory => inventory.BagSlotIndex == inventoryBag.SlotIndex).ToList();

                    // Init all items for this bag.
                    inventoryBag.LoadItems(thisBagsItems);
                }
            }
        }
    }
}
