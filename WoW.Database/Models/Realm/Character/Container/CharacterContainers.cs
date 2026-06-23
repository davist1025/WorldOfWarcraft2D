using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Database.Models.Realm.Character.Container
{
    [Table("character_containers")]
    public class CharacterContainers
    {
        [Column("character_id")]
        public int CharacterId { get; set; }

        [Column("bag_index_id")]
        public int BagIndice { get; set; } // 0-4; 0 being the main default bag. 

        [Column("bag_item_id")]
        public int BagItemId { get; set; }
    }
}
