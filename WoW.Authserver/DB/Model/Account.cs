using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Server.Shared.Vocab;

namespace WoW.Authserver.DB.Model
{
    [Table("accounts")]
    public class Account
    {
        public int Id { get; set; }

        public string Username { get; set; }
        
        public string? SessionId { get; set; }

        [NotMapped]
        public SecurityLevel Security => (SecurityLevel)SecurityId;

        public int SecurityId { get; set; }
    }
}
