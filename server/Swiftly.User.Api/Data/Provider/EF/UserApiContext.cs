using Microsoft.EntityFrameworkCore;
using Swiftly.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftly.User.Api.Data.Provider.EF
{
    /// <summary>
    /// The EF context for user API
    /// </summary>
    internal class UserApiContext : DbContext
    {
        private readonly ISwiftlyDbConfigurer mConfigurer;
        
        public UserApiContext(ISwiftlyDbConfigurer configurer)
        {
            mConfigurer = configurer;
        }

        /// <summary>
        /// The list of the users
        /// </summary>
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            mConfigurer.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasIndex(m => m.UserName).IsUnique(true);
            modelBuilder.Entity<UserEntity>()
                .HasIndex(m => m.Role);
            modelBuilder.Entity<UserEntity>()
                .HasIndex(m => m.Status);
        }
    }
}
