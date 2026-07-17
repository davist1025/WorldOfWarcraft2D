using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Database.Models.Auth
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Stored in all capital letters.
        /// </summary>
        [Column("account_name", TypeName = "varchar(64)")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Username { get; set; }

        [Column("password_hash", TypeName = "varchar(128)")]
        public string HashedPassword { get; set; }

        [Column("email", TypeName = "varchar(50)")]
        public string? Email { get; set; }

        [Column("session_id", TypeName = "varchar(32)")]
        public string SessionId { get; set; }

        [Column("user_security")]
        public int SecurityLevel { get; set; }

        [NotMapped]
        public AccountSecurityType Security => (AccountSecurityType)SecurityLevel;
    }
}
