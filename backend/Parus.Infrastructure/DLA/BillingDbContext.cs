using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parus.Core.Entities;
using Parus.Infrastructure.Identity;
using System.Diagnostics;

namespace Parus.Infrastructure.DLA
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext()
        {

        }

        public DbSet<SubscriptionProfile> SubscriptionProfiles { get; set; }
        public DbSet<SubscriptionSession> SubscriptionSessions { get; set; }

        private string _connectionString = "Data Source=192.168.100.11;Database=Parus;User ID=ivan;Password=zx12;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public BillingDbContext(DbContextOptions<BillingDbContext> options, string connectionString = "")
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SubscriptionProfile>()
                .HasMany(x => x.Sessions)
                .WithOne(x => x.Profile);

            builder.Entity<SubscriptionProfile>()
                .HasIndex(x => x.Name)
                .IsClustered(false);

            builder.Entity<SubscriptionProfile>()
                .Property(x => x.Name)
                .IsRequired(true);

            builder.Entity<SubscriptionProfile>()
                .Property(x => x.PriceUnit)
                .HasDefaultValue(1);

            builder.Entity<SubscriptionProfile>()
                .Property(x => x.DurationDays)
                .HasDefaultValue(30);

            builder.Entity<SubscriptionProfile>()
                .HasKey(x => x.SubscriptionProfileId);



            builder.Entity<SubscriptionSession>()
                .HasKey(x => x.SubscriptionSessionId);

            builder.Entity<SubscriptionSession>()
                .Property(x => x.Status)
                .HasDefaultValue(SubscriptionSessionStatus.NonActive);

            builder.Entity<SubscriptionSession>()
                .Property(x => x.ProfileId)
                .IsRequired();

            builder.Entity<SubscriptionSession>()
                .Property(x => x.Autocontinuation)
                .HasDefaultValue(false);

            builder.Entity<SubscriptionSession>()
                .Property(x => x.BroadcastId)
                .IsRequired();

            builder.Entity<SubscriptionSession>()
                .Property(x => x.ProfileId)
                .IsRequired();

            //            builder.Entity<SubscriptionProfile>().HasIndex();

            //.HasOne(x => x.Profile);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!String.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}
