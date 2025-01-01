using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.DB.Model
{
    public enum NpcBehaviorType
    {
        Aggressive = 1,
        Passive,
        Scripted,
    }

    [Table("npc_behavior")]
    public class NPC_Behavior
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("npc_id")]
        public int NpcId { get; set; }

        [Column("behavior_type")]
        public int BehaviorType { get; set; }

        /// <summary>
        /// The name of the script this behavior uses.
        /// 
        /// TODO: Currently unused, but will be used when behavior scripts are implemented. 
        /// </summary>
        [Column("script")]
        public string? Script { get; set; }
    }
}
