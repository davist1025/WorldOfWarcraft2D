using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Authserver.DB.Model
{
    [Table("realmlist")]
    public class Realmserver
    {
        [Key]
        [Column("realm_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("realm_name", TypeName = "varchar(32)")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get; set; }

        [Column("hostname", TypeName = "varchar(64)")]
        public string Hostname { get; set; }

        [Column("port")]
        public int Port { get; set; }

        [Column("flags")]
        public int? Flag { get; set; }

        [NotMapped]
        public RealmFlags Flags => (RealmFlags)Flag;

        [NotMapped]
        public IPEndPoint StoredEndPoint => new IPEndPoint(IPAddress.Parse(Hostname), Port);
    }

    [Flags]
    public enum RealmFlags
    {
        IsPTR = 1,
        IsRestricted = 2
    }
}
