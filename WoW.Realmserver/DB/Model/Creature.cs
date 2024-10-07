using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;

namespace WoW.Realmserver.DB.Model
{
    [Table("creatures")]
    public class Creature
    {
        public int Id { get; set; }
        public string? RawId { get; set; }
        public string Name { get; set; }
        public string? SubName { get; set; }
        public string DisplayId { get; set; }
        public string ScriptId { get; set; }

        [Obsolete("Not implemented.")]
        public string BehaviorId { get; set; }

        /// <summary>
        /// Describes a target in which the player can "tab" to.
        /// Mailboxes, quest objects, etc are excluded from this.
        /// </summary>
        public bool IsTargetable { get; set; }
        
        /// <summary>
        /// Will this Creature automatically target and attack any player in "sight?"
        /// </summary>
        public bool IsAggressive { get; set; }

        [NotMapped]
        public GameObjectFlags Flags => (GameObjectFlags)FlagsLiteral;

        [Column("Flags")]
        public int FlagsLiteral { get; set; }
    }
}
