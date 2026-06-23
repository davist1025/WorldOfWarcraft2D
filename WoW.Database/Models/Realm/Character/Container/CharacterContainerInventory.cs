using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Database.Models.Realm.Character.Container
{
    [Table("character_container_inventory")]
    public class CharacterContainerInventory
    {
        [Column("character_id")]
        public int CharacterId { get; set; }

        [Column("bag_index_id")]
        public int BagIndice { get; set; } // 0-4; 0 being the main default bag.

        /// <summary>
        /// The slot id of the bag a given item is stored in; 0-bag_size.
        /// </summary>
        [Column("bag_slot_index")]
        public int BagSlotIndex { get; set; }

        /// <summary>
        /// Id of the item stored within a bag slot.
        /// </summary>
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("item_count")]
        public int StackCount { get; set; }
    }
}
