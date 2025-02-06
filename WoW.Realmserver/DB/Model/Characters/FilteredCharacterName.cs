using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.DB.Model.Characters
{
    [Table("character_name_filter")]
    public class FilteredCharacterName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int NameOrPhraseId { get; set; }

        [Column("phrase")]
        public string NameOrPhrase { get; set; }
    }
}
