using LiteDB;
using LiteDB.Engine;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Naturistic.Infrastructure.Identity
{
    public class LiteDbIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public LiteDatabase LiteDbDatabase { get; }
        
        public LiteDbIdentityContext(DbContextOptions<LiteDbIdentityContext> options) 
            : base(options) 
        {
            //IOptions<LiteDbOptions> ldOptions
            //LiteDbDatabase = new LiteDatabase(ldOptions.Value.DatabaseLocation);
        }
    }
}