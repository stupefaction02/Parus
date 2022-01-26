// using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Naturistic.Infrastructure.Identity
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        // public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
        //     : base(options)
        // {
        //     
        // }

        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
            : base(options)
        {
            
        }

        public ApplicationIdentityDbContext()
        { 
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // # Data Source - SELECT @@SERVERNAME AS 'Server Name' in sqlcmd ;)
                string connectionString =
                    "Data Source=fd;Database=naturistic_users;User Id=sa;Password=!Kronos39!;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
       
    }
}
