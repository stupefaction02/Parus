// using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit.Encodings;
using Org.BouncyCastle.Tls;
using Parus.Core.Entities;
using Parus.Infrastructure.DataLayer;
using static Parus.Infrastructure.DataLayer.ApplicationDbContext;

namespace Parus.Infrastructure.Identity
{
    public enum ConnectionType
    {
        MSSQL,
        Postgres
    }

    public class SampleContextFactory : IDesignTimeDbContextFactory<ParusDbContext>
    {
        public ParusDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ParusDbContext>();

            return new ParusDbContext(optionsBuilder.Options);
        }
    }

    public class ParusDbContext : IdentityDbContext<ParusUser>
    {
        private ConnectionType _connectionType = ConnectionType.MSSQL;

        #region Billing 

        public DbSet<SubscriptionProfile> SubscriptionProfiles { get; set; }

        public DbSet<SubscriptionSession> SubscriptionSessions { get; set; }

        #endregion

        #region Broadcasts

        public DbSet<Broadcaster> Broacasters { get; set; }

        public DbSet<Broadcast> Broadcasts { get; set; }

        public DbSet<BroadcastInfoKeyword> BroadcastsKeywords { get; set; }

        public DbSet<BroadcastTag> Tags { get; set; }

        public DbSet<BroadcastCategory> Categories { get; set; }

        #endregion

        #region Identity

        public DbSet<RefreshSession> RefreshSessions { get; set; }
        public DbSet<ConfirmCode> ConfirmCodes { get; set; }
        public DbSet<PasswordRecoveryToken> PasswordRecoveryTokens { get; set; }

        public DbSet<TwoFactoryEmailVerificationCode> TwoFactoryVerificationCodes { get; set; }
        
        // Description/Comment: keys is created when user scans qr_code and send numbers to the server
        public DbSet<TwoFactoryCustomerKey> TwoFactoryCustomerKeys { get; set; }

        private string _connectionString = "Data Source=192.168.100.11;Database=Parus;User ID=ivan;Password=zx12;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public ParusDbContext(DbContextOptions<ParusDbContext> options, string connectionString = "", ConnectionType connectionType = ConnectionType.MSSQL)
            : base(options)
        {
            _connectionString = connectionString;
            _connectionType = connectionType;
        }

        #endregion

        public ParusDbContext(DbContextOptions<ParusDbContext> options) : base(options)
        {

        }

        public ParusDbContext()
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Broadcast>()
                .Property(x => x.IndexingStatus)
                .HasDefaultValue(1);

            builder.Entity<BroadcastCategory>()
                .Property(x => x.IndexingStatus)
                .HasDefaultValue(1);




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
                .Property(x => x.BroadcasterId)
                .IsRequired();

            builder.Entity<SubscriptionSession>()
                .Property(x => x.ProfileId)
                .IsRequired();


            builder.Entity<Broadcaster>()
                .HasKey(x => x.BroadcasterId);

            builder.Entity<ParusUser>()
                .HasOne(x => x.Broadcaster)
                .WithOne(x => x.Owner)
                .HasForeignKey<Broadcaster>(x => x.OwnerId);

            //builder.Entity<Broadcaster>()
            //    .HasOne(x => x.Owner)
            //    .WithOne(x => x.Broadcaster);
            //.HasForeignKey<ApplicationUser>(x => x.);

            builder.Entity<ParusSubscriptionSession>()
                .HasOne(x => x.Broacaster)
                .WithMany(x => x.SubscriptionSessionsAsSubject)
                .HasForeignKey(x => x.BroadcasterId);




            builder.Entity<ParusUser>()
                    .HasOne(e => e.ConfirmCode)
                    .WithOne(e => e.User)
                    .HasForeignKey<ConfirmCode>(e => e.UserId)
                    .IsRequired();

            builder.Entity<ParusUser>()
                    .HasOne(e => e.TwoFAEmailVerificationCode)
                    .WithOne(e => e.User)
                    .HasForeignKey<TwoFactoryEmailVerificationCode>(e => e.UserId)
                    .IsRequired();

            //builder.Entity<TwoFactoryCustomerKey>().Property(x => x.UserId);

            builder.Entity<ParusUser>()
                    .HasOne(e => e.CustomerKey)
                    .WithOne(e => e.User)
                    .HasForeignKey<TwoFactoryCustomerKey>(e => e.UserId);

            builder.Entity<ParusUser>()
                    .Property(x => x.AvatarPath)
                    .HasDefaultValue("defaults/ava1.jpg");

            builder.Entity<Broadcast>()
                    .Property(x => x.Preview)
                    .HasDefaultValue("defaults/preview_bright.jpg");

            builder.Entity<ParusUser>()
                    .HasOne(e => e.PasswordRecoveryToken)
                    .WithOne(e => e.User)
                    .HasForeignKey<PasswordRecoveryToken>(e => e.UserId)
                    .IsRequired();

            builder.Entity<ParusUser>().Property(x => x.UserName).IsRequired(true);
            builder.Entity<ConfirmCode>().Property(x => x.UserId).IsRequired(true);

            builder.Entity<ParusUser>()
                .Property(x => x.IndexingRule)
                .HasDefaultValue(1);

            // Schems 

            builder.Entity<ParusUser>()
                .ToTable("AspNetUsers", "Identity");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!String.IsNullOrEmpty(_connectionString))
            {
                ConfigServer(_connectionString, optionsBuilder);
            }
        }

        public void ConfigServer(string connectionString, DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionType == ConnectionType.MSSQL)
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");
                optionsBuilder.UseSqlServer(_connectionString);
            }
            else if (_connectionType == ConnectionType.Postgres)
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }
    }
}
