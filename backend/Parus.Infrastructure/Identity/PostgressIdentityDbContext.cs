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
using MimeKit.Encodings;
using Org.BouncyCastle.Tls;
using Parus.Core.Entities;
using Parus.Infrastructure.DLA;

namespace Parus.Infrastructure.Identity
{
    public class SampleContextFactory1 : IDesignTimeDbContextFactory<PostgressIdentityDbContext>
    {
        public PostgressIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PostgressIdentityDbContext>();

            return new PostgressIdentityDbContext(optionsBuilder.Options);
        }
    }
    public class PostgressIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<RefreshSession> RefreshSessions { get; set; }
        public DbSet<ConfirmCode> ConfirmCodes { get; set; }
        public DbSet<PasswordRecoveryToken> PasswordRecoveryTokens { get; set; }

        public DbSet<TwoFactoryEmailVerificationCode> TwoFactoryVerificationCodes { get; set; }
        
        // Description/Comment: keys is created when user scans qr_code and send numbers to the server
        public DbSet<TwoFactoryCustomerKey> TwoFactoryCustomerKeys { get; set; }

        //private string _connectionString = "postgresql://localhost:5432/identity";
        private string _connectionString = "Data Source=localhost;location=identity;User ID=postgres;password=zx12";
        public PostgressIdentityDbContext(DbContextOptions<PostgressIdentityDbContext> options, string connectionString = "")
            : base(options)
        {
            _connectionString = connectionString;
        }

        public PostgressIdentityDbContext()
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
            if (!String.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationIdentityDbContext)}");
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}
