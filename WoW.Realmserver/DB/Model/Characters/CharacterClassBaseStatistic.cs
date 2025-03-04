using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.DB.Model.Characters
{
    [Table("character_class_stats")]
    public class CharacterClassBaseStatistic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("class_id")]
        public int ClassId { get; set; }

        [Column("strength_amt")]
        public int Strength { get; set; }

        [Column("agility_amt")]
        public int Agility { get; set; }

        [Column("intellect_amt")]
        public int Intellect { get; set; }

        [Column("stamina_amt")]
        public int Stamina { get; set; }

        [Column("spirit_amt")]
        public int Spirit { get; set; }
    }
}
