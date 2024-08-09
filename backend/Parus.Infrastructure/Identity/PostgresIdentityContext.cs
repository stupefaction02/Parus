// using System;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Parus.Core.Entities;

namespace Parus.Infrastructure.Identity
{
    public class SampleContextFactory1 : IDesignTimeDbContextFactory<PostgresIdentityContext>
    {
        public PostgresIdentityContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PostgresIdentityContext>();

            return new PostgresIdentityContext(optionsBuilder.Options);
        }
    }
    public class PostgresIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<RefreshSession> RefreshSessions { get; set; }
        public DbSet<ConfirmCode> ConfirmCodes { get; set; }
        public DbSet<PasswordRecoveryToken> PasswordRecoveryTokens { get; set; }

        public DbSet<TwoFactoryEmailVerificationCode> TwoFactoryVerificationCodes { get; set; }

        // Description/Comment: keys is created when user scans qr_code and send numbers to the server
        public DbSet<TwoFactoryCustomerKey> TwoFactoryCustomerKeys { get; set; }

        private string _connectionString;
        public PostgresIdentityContext(DbContextOptions<PostgresIdentityContext> options, string connectionString = "")
            : base(options)
        {
            _connectionString = "User ID=postgres;Password=zx1;Host=localhost;Port=5432;Database=identity;Pooling=true;Connection Lifetime=0;";
        }

        public PostgresIdentityContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
            optionsBuilder.UseNpgsql(_connectionString); return;
            if (!String.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ParusDbContext)}");
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }
    }
}
