using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WoW.Realmserver.DB.Model;

namespace WoW.Realmserver.DB
{
    internal class RealmContext : DbContext
    {
        public DbSet<PlayerCharacter> Characters { get; set; }
        public DbSet<Creature> Creatures { get; set; }

        private const string _connectionString = "server=127.0.0.1;uid=root;pwd=1111;database=wpp_realm";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerCharacter>()
                .HasKey(p => new { p.AccountId });

            modelBuilder.Entity<PlayerCharacter>()
                .Property(p => p.AccountId)
                .IsRequired();

            modelBuilder.Entity<PlayerCharacter>()
                .Property(p => p.CharacterId)
                .IsRequired();

            modelBuilder.Entity<PlayerCharacter>()
                .Property(p => p.ClassId)
                .IsRequired();

            modelBuilder.Entity<PlayerCharacter>()
                .Property(p => p.Name)
                .IsRequired();

            modelBuilder.Entity<PlayerCharacter>()
                .Property(p => p.RaceId)
                .IsRequired();

            modelBuilder.Entity<PlayerCharacter>()
                .Property(p => p.Level)
                .IsRequired();

            /*       */

            modelBuilder.Entity<Creature>()
                .HasKey(p => new { p.Id });

            modelBuilder.Entity<Creature>()
                .Property(p => p.Id)
                .UseMySqlIdentityColumn();

            modelBuilder.Entity<Creature>()
                .Property(p => p.Name)
                .IsRequired();
        }
    }
}
