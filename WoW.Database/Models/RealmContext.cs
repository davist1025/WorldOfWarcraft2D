using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models.Realm;
using WoW.Database.Models.Realm.Character;
using WoW.Database.Models.Realm.Chat;

namespace WoW.Database.Models
{
    public class RealmContext : DbContext
    {
        public DbSet<PlayerCharacter> Characters { get; set; }
        public DbSet<PlayerCharacterStatistic> CharacterStatistics { get; set; }
        public DbSet<FilteredCharacterName> CharacterNameFilters { get; set; }
        public DbSet<CharacterClassBaseStatistic> BaseClassStatistics { get; set; }

        public DbSet<NonPlayerCharacter> NPCs { get; set; }
        public DbSet<NonPlayerCharacterBehavior> NpcBehaviors { get; set; }

        public DbSet<ChatCommand> Commands { get; set; }
        public DbSet<ChatCommandChild> ChildCommands { get; set; }

        public DbSet<CharacterRaceSpawn> RaceSpawns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string hostname = EnvironmentContext.AppSettings["db_hostname"];
            string password = EnvironmentContext.AppSettings["db_password"];
            string uid = "root";
            string db = EnvironmentContext.AppSettings["database"];

            optionsBuilder.UseMySql($"server={hostname};uid={uid};pwd={password};database={db}", ServerVersion.AutoDetect($"server={hostname};uid={uid};pwd={password};database={db}"));
        }
    }
}
