using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Server.Shared.Database.Model.Auth;
using static WoW.Server.Shared.Vocab;

namespace WoW.Server.Shared.Database.Model
{
    /// <summary>
    /// Database context for the authserver tables.
    /// </summary>
    public class AuthContext : DbContext
    {
        public DbSet<Realmserver> Realmlist { get; set; }
        public DbSet<Account> Accounts { get; set; }

        // todo: make this global. currently there are like 3-4 instances??
        private const string _connectionString = "server=127.0.0.1;uid=root;pwd=1111;database=wpp_auth";

        private DbContextOptions<AuthContext> _options;

        public AuthContext() { }

        public AuthContext(DbContextOptions<AuthContext> options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }
    }
}
