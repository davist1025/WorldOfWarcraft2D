using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Server.Shared.Database.Model.Realm.Chat
{
    [Table("chat_command")]
    public class ChatCommand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("command_id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("help_text")]
        public string? HelpText { get; set; }

        [Column("security_level")]
        public int Security { get; set; }

        [Column("handler_id")]
        public string? HandlerId { get; set; }
    }
}
