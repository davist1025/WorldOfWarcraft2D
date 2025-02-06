using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.DB.Model.Characters
{
    [Table("character_race_spawn")]
    public class CharacterRaceSpawn
    {
        [Key]
        [Column("race_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RaceId { get; set; }

        [Column("map_id")]
        public string MapId { get; set; }

        [Column("x_position")]
        public float X { get; set; }

        [Column("y_position")]
        public float Y { get; set; }
    }
}
