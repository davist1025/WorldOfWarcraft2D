using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;

namespace WoW.Realmserver.DB.Model
{
    /// <summary>
    /// Contains base information for a player character.
    /// </summary>
    [Table("characters")]
    public class PlayerCharacter
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }
        public int CharacterId { get; set; }
        public int GuildId { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public RaceType Race => (RaceType)RaceId;

        public int RaceId { get; set; }

        [NotMapped]
        public CharacterClassType Class => (CharacterClassType)ClassId;

        public int ClassId { get; set; }

        public int Level { get; set; }

        // todo: add a function to create serializable versions of model objects.
    }
}
