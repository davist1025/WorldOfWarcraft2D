using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Database.Models.Realm.Items
{
    /// <summary>
    /// Defines items held in any given bag on a character.
    /// </summary>
    [Table("character_bag_inventory")]
    public class CharacterBagInventoryIndex
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("entry_id")]
        public int EntryId { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("character_id")]
        public int CharacterId { get; set; }

        [Column("bag_slot_index")]
        public int BagSlotIndex { get; set; }

        [Column("bag_space_index")]
        public int BagSpaceIndex { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("item_stack_count")]
        public int ItemStackCount { get; set; }
    }
}
