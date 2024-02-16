using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Parus.Core.Entities;

namespace Parus.Infrastructure.DLA
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;
        #region System
        public ApplicationDbContext()
        {
            
        }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            this.options = options;
        }
		#endregion

		#region Tables

		public DbSet<BroadcastInfo> Broadcasts { get; set; }

        public DbSet<BroadcastInfoKeyword> BroadcastsKeywords { get; set; }

        public DbSet<Tag> Tags { get; set; }

		public DbSet<BroadcastCategory> Categories { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }


        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// move to config
            //int titeLength = 250;
            //modelBuilder.Entity<BroadcastInfo>().Property(x => x.Title).IsRequired().HasMaxLength(titeLength);

            ConfigureRelations(modelBuilder);
        }

		private void ConfigureRelations(ModelBuilder modelBuilder)
		{

		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                // # Data Source - SELECT @@SERVERNAME AS 'Server Name' in sqlcmd ;)
                string connectionString =
                    "Data Source=DESKTOP-OTM8VD2;Database=Naturistic.BL;TrustServerCertificate=True;Integrated Security=True;";
                //"Data Source=DESKTOP-OTM8VD2;Database=Naturistic.BL;User Id=sa;Password=!Kronos39!;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}