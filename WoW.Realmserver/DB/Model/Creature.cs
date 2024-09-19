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
        public string Name { get; set; }
        public string? SubName { get; set; }
        public string DisplayId { get; set; }

        [NotMapped]
        public GameObjectFlags Flags => (GameObjectFlags)FlagsLiteral;

        [Column("Flags")]
        public int FlagsLiteral { get; set; }
    }
}
