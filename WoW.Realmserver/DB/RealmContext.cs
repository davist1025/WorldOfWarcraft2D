using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WoW.Realmserver.DB.Model;
using WoW.Realmserver.DB.Model.Chat;

namespace WoW.Realmserver.DB
{
    internal class RealmContext : DbContext
    {
        public DbSet<PlayerCharacter> Characters { get; set; }
        public DbSet<NonPlayerCharacter> NPCs { get; set; }
        public DbSet<NPC_Behavior> Behaviors { get; set; }

        public DbSet<ChatCommand> Commands { get; set; }
        public DbSet<ChatCommandChild> ChildCommands { get; set; }

        private const string _connectionString = "server=127.0.0.1;uid=root;pwd=1111;database=wpp_realm";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }
    }
}
