using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Parus.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Parus.API.Tests
{
    public class ParusDbContextSeeder
    {
        private ParusDbContext _context;
        private readonly ServiceProvider serviceProvider;

        public List<Action> SeedActions { get; internal set; } = new();

        public ParusDbContextSeeder(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void PurgeAndSeed()
        {
            if (_context == null)
            {
                _context = serviceProvider.GetRequiredService<ParusDbContext>();
                //_context = serviceProvider.GetRequiredService<IPasswordHasher<ParusUser>>();
            }

            Purge(_context);

            Seed(_context);
            
            _context.SaveChanges();
        }

        protected void Seed(ParusDbContext context)
        {
            
        }

        protected void Purge(ParusDbContext context)
        {
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[dbo].[RefreshSessions]");
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[dbo].[Broadcasts]");
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[dbo].[Broadcasters]");
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[Identity].[AspNetUsers]");
        }

        public void Purge()
        {

        }
    }
}