using Microsoft.EntityFrameworkCore; 
using Naturistic.Core.Entities;

namespace Naturistic.Infrastructure.DLA
{
    public class ApplicationDbContext : DbContext
    {
		public DbSet<BroadcastInfo> Broadcasts;
		
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}