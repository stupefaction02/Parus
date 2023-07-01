using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; 
using Naturistic.Core.Entities;

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

        public DbSet<ViewerUser> ViewerUsers { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<BroadcastUser> BroadcastUsers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BroadcastUser>()
                        .HasOne(x => x.Chat)
                        .WithOne(x => x.BroadcastUser)
                        .HasForeignKey<Chat>(x => x.ChatId);

            // modelBuilder.Entity<Message>()
            //             .HasOne(x => x.Sender)
            //             .WithOne(x => x.)
            //             .HasForeignKey<ViewerUser>(x => x.);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (true)//!optionsBuilder.IsConfigured)
            {
                // # Data Source - SELECT @@SERVERNAME AS 'Server Name' in sqlcmd ;)
                string connectionString =
                    "Data Source=DESKTOP-OTM8VD2;Database=Naturistic.BL";
                //"Data Source=DESKTOP-OTM8VD2;Database=Naturistic.BL;User Id=sa;Password=!Kronos39!;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}