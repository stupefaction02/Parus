// using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Naturistic.Infrastructure.Identity
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

            var testUsers = new ApplicationUser[3] {
                new ApplicationUser { UserName = "Guts" },
                new ApplicationUser { UserName = "Griffith" },
                new ApplicationUser { UserName = "Farzana" }
            };
            builder.Entity<ApplicationUser>().HasData(testUsers);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                    "Data Source=DESKTOP-OTM8VD2;Database=Naturistic.Users;TrustServerCertificate=True;Integrated Security=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
