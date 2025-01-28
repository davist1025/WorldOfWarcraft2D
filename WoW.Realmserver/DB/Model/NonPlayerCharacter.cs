using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.DB.Model
{
    [Table("npc")]
    public class NonPlayerCharacter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Utilized by the end user to determine what sprite/animation to draw while an instance of this NPC is in the game world.
        /// </summary>
        [Column("model_id")]
        public string ModelId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("npc_flag")]
        public int FlagType { get; set; }

        [Column("level")]
        public int Level { get; set; } = 1;
    }
}
