using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components
{
    /// <summary>
    /// Houses both equipment and bag container functionality.
    /// </summary>
    public class InventoryComponent : Component
    {
        private List<BagInventory> _bags = new List<BagInventory>();
        public BagInventory[] Bags => _bags.ToArray();

        public void EquipBag(int indice, int bagItemId)
        {
            if (_bags.Count < 4)
                _bags.Add(new BagInventory(indice, bagItemId));
        }
    }

    public class BagInventory
    {
        private int _equippedIndice = -1;
        public int EquippedIndice => _equippedIndice;

        private int _bagItemId = -1;
        public int ThisId => _bagItemId;

        public int TotalSlotCount = -1;

        public BagInventory(int indice, int bagItemId)
        {
            _equippedIndice = indice;
            _bagItemId = bagItemId;
        }
    }
}
