using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Database.Models.Realm
{
    [Table("npc_behavior")]
    public class NonPlayerCharacterBehavior
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("npc_id")]
        public int NpcId { get; set; }

        /// <summary>
        /// The name of the script this behavior uses.
        /// </summary>
        [Column("behavior_code_id")]
        public string? Script { get; set; }
    }
}
