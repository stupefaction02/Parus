using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Parus.Core.Entities;

namespace Parus.Infrastructure.DataLayer
{
    public class PostgressCoreContext : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;
        #region System
        public PostgressCoreContext()
        {

        }
        #endregion

        private string _connectionString;
        public PostgressCoreContext(DbContextOptions<PostgressCoreContext> options, string connectionString = "") : base(options)
        {
            _connectionString = "User ID=postgres;Password=zx1;Host=localhost;Port=5432;Database=bl;Pooling=true;Connection Lifetime=0;";
        }

        #region Tables

        public DbSet<Broadcast> Broadcasts { get; set; }

        public DbSet<BroadcastInfoKeyword> BroadcastsKeywords { get; set; }

        public DbSet<BroadcastTag> Tags { get; set; }

        public DbSet<BroadcastCategory> Categories { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }


        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //// move to config
            //int titeLength = 250;
            //modelBuilder.Entity<BroadcastInfo>().Property(x => x.Title).IsRequired().HasMaxLength(titeLength);

            builder.Entity<Broadcast>()
                .Property(x => x.IndexingStatus)
                .HasDefaultValue(1);

            builder.Entity<BroadcastCategory>()
                .Property(x => x.IndexingStatus)
                .HasDefaultValue(1);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            if (!string.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }
    }
}