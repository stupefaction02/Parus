// using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Parus.Core.Entities;

namespace Parus.Infrastructure.Identity
{
    public class SampleContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();

            return new ApplicationIdentityDbContext(optionsBuilder.Options);
        }
    }

    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<RefreshSession> RefreshSessions { get; set; }
        public DbSet<ConfirmCode> ConfirmCodes { get; set; }
        public DbSet<PasswordRecoveryToken> PasswordRecoveryTokens { get; set; }

        public DbSet<TwoFactoryEmailVerificationCode> TwoFactoryVerificationCodes { get; set; }
        
        // Description/Comment: keys is created when user scans qr_code and send numbers to the server
        public DbSet<TwoFactoryCustomerKey> TwoFactoryCustomerKeys { get; set; }

        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
            : base(options)
        {
            //if (Database.EnsureCreated()) Database.Migrate();
        }

        public ApplicationIdentityDbContext()
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

            //builder.Entity<ApplicationUser>().Property(x => x.UserName).IsRequired();

            //var testUsers = new ApplicationUser[3] {
            //    new ApplicationUser { UserName = "Guts" },
            //    new ApplicationUser { UserName = "Griffith" },
            //    new ApplicationUser { UserName = "Farzana" }
            //};
            //builder.Entity<ApplicationUser>().HasData(testUsers);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                    "Data Source=DESKTOP-OTM8VD2;Database=Naturistic.Users;TrustServerCertificate=True;Integrated Security=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
