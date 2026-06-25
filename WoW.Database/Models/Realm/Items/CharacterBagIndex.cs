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
    /// Defines which bags are equipped.
    /// </summary>
    [Table("character_bag_index")]
    [Keyless]
    public class CharacterBagIndex
    {
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("character_id")]
        public int CharacterId { get; set; }

        [Column("bag_slot_index")]
        public int BagSlotIndex { get; set; }

        [Column("bag_item_id")]
        public int BagItemId { get; set; }
    }
}
