// using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Parus.Infrastructure.DLA;
using static Parus.Infrastructure.DLA.ApplicationDbContext;

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

    public class ParusDbContext : IdentityDbContext<ApplicationUser>
    {
        private ConnectionType _connectionType = ConnectionType.MSSQL;

        #region Billing 

        public DbSet<SubscriptionProfile> SubscribeProfiles { get; set; }

        public DbSet<SubscriptionSession> SubscribeSessions { get; set; }

        #endregion

        public DbSet<RefreshSession> RefreshSessions { get; set; }
        public DbSet<ConfirmCode> ConfirmCodes { get; set; }
        public DbSet<PasswordRecoveryToken> PasswordRecoveryTokens { get; set; }

        public DbSet<TwoFactoryEmailVerificationCode> TwoFactoryVerificationCodes { get; set; }
        
        // Description/Comment: keys is created when user scans qr_code and send numbers to the server
        public DbSet<TwoFactoryCustomerKey> TwoFactoryCustomerKeys { get; set; }

        private string _connectionString;
        public ParusDbContext(DbContextOptions<ParusDbContext> options, string connectionString = "", ConnectionType connectionType = ConnectionType.MSSQL)
            : base(options)
        {
            _connectionString = connectionString;
            _connectionType = connectionType;
        }

        public ParusDbContext()
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //base.Database.

            builder.Entity<ApplicationUser>()
                    .HasOne(e => e.ConfirmCode)
                    .WithOne(e => e.User)
                    .HasForeignKey<ConfirmCode>(e => e.UserId)
                    .IsRequired();

            builder.Entity<ApplicationUser>()
                    .HasOne(e => e.TwoFAEmailVerificationCode)
                    .WithOne(e => e.User)
                    .HasForeignKey<TwoFactoryEmailVerificationCode>(e => e.UserId)
                    .IsRequired();

            //builder.Entity<TwoFactoryCustomerKey>().Property(x => x.UserId);

            builder.Entity<ApplicationUser>()
                    .HasOne(e => e.CustomerKey)
                    .WithOne(e => e.User)
                    .HasForeignKey<TwoFactoryCustomerKey>(e => e.UserId);

            builder.Entity<ApplicationUser>()
                    .Property(x => x.AvatarPath)
                    .HasDefaultValue("defaults/ava1.jpg");

            builder.Entity<BroadcastInfo>()
                    .Property(x => x.Preview)
                    .HasDefaultValue("defaults/preview_bright.jpg");

            builder.Entity<ApplicationUser>()
                    .HasOne(e => e.PasswordRecoveryToken)
                    .WithOne(e => e.User)
                    .HasForeignKey<PasswordRecoveryToken>(e => e.UserId)
                    .IsRequired();

            builder.Entity<ApplicationUser>().Property(x => x.UserName).IsRequired(true);
            builder.Entity<ConfirmCode>().Property(x => x.UserId).IsRequired(true);

            builder.Entity<ApplicationUser>()
                .Property(x => x.IndexingRule)
                .HasDefaultValue(1);
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
