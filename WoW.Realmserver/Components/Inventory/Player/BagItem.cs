using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Components.Inventory.Player
{
    /// <summary>
    /// Defines an item within any given bag.
    /// </summary>
    public class BagItem
    {
        public int BagSpaceIndex { get; private set; }
        public int ItemId { get; private set; }
        public int StackCount { get; private set; }

        public BagItem(int bagSpaceIndex, int itemId, int stackCount)
        {
            BagSpaceIndex = bagSpaceIndex;
            ItemId = itemId;
            StackCount = stackCount;
        }
    }
}
