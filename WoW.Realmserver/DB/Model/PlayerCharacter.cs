using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;

namespace WoW.Realmserver.DB.Model
{
    /// <summary>
    /// Contains base information for a player character.
    /// </summary>
    [Table("characters")]
    public class PlayerCharacter
    {
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("character_id")]
        public int CharacterId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("character_name", TypeName = "varchar(16)")]
        public string Name { get; set; }

        [Column("character_race_id")]
        public int RaceId { get; set; }

        [Column("character_hair_id")]
        public int HairId { get; set; } = 1;

        [Column("zone_x_position")]
        public float XPosition { get; set; }

        [Column("zone_y_position")]
        public float YPosition { get; set; }

        [Column("map_id")]
        public string MapId { get; set; }

        [Column("animation_direction")]
        public int Direction { get; set; }
    }
}
