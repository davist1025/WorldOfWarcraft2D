using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Authserver.DB.Model;
using static WoW.Server.Shared.Vocab;

namespace WoW.Authserver.DB
{
    /// <summary>
    /// Database context for the authserver tables.
    /// </summary>
    public class AuthContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        // todo: we might be able to allow a host to set this in a production context, since the database is automatically migrated upon launch.
        private const string _connectionString = "server=127.0.0.1;uid=root;pwd=1111;database=wpp_auth";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasKey(p => new { p.Id });

            modelBuilder.Entity<Account>()
                .Property(p => p.Id)
                .UseMySqlIdentityColumn();

            modelBuilder.Entity<Account>()
                .Property(p => p.Username)
                .IsRequired();
        }
    }
}
