using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; 
using Naturistic.Core.Entities;
using Naturistic.Core.Entities.Chat;

namespace Naturistic.Infrastructure.DLA
{
    public class ApplicationDbContext : DbContext
    {
		public DbSet<BroadcastInfo> Broadcasts;

        public ApplicationDbContext()
        {
            
        }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        }

        public List<Message> Messages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}