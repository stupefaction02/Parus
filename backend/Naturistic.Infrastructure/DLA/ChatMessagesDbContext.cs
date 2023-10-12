using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore; 
using Naturistic.Core.Entities;

namespace Naturistic.Infrastructure.DLA
{
    public class ChatMessagesDbContext : DbContext
    {
        #region System
        public ChatMessagesDbContext()
        {

        }

        public ChatMessagesDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                    "Data Source=DESKTOP-OTM8VD2;Database=Naturistic.ChatMessages;TrustServerCertificate=True;Integrated Security=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}