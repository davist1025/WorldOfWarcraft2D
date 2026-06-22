using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Database.Models.Realm
{
    /// <summary>
    /// Describes a game item in the database.
    /// </summary>
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Utilized by the end-user to define what image to draw in the player's inventory.
        /// </summary>
        [Column("inventory_modelid")]
        public string Inventory_ModelId { get; set; }

        /// <summary>
        /// The image seen in the game world, if any. This would often apply to some armor pieces, weapon, etc.
        /// </summary>
        [Column("rendered_modelid")]
        public string Rendered_ModelId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("itemtype_id")]
        public ItemType Descriptor { get; set; }
    }

    public enum ItemType
    {
        Quest_Reward,
        Quest_Requirement,
        Weapon_1H, Weapon_2H,
        Ammo_Arrow,
        Ammo_Bullet,
        Ammo_Thrown,
        Book,
        Armor_Head,
        Armor_Shoulder,
        Armor_Chest,
        Armor_Wrists,
        Armor_Hands,
        Armor_Waist,
        Armor_Legs,
        Armor_Feet,
        Accessory_Finger,
        Accessory_Trinket,
        Accessory_Neck,
        Consumable,
        Storage_Small
    }
}
