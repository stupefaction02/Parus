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

        public DbSet<SubscribeProfile> SubscribeProfiles { get; set; }
        public DbSet<SubscribeSession> SubscribeSessions { get; set; }

        private string _connectionString = "Data Source=192.168.100.11;Database=Parus.Billing;User ID=ivan;Password=zx12;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public BillingDbContext(DbContextOptions<BillingDbContext> options, string connectionString = "")
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
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
