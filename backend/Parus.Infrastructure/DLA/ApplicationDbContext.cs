﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Parus.Core.Entities;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DLA
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;
        #region System
        public ApplicationDbContext()
        {
            
        }
        #endregion

        private string _connectionString;
        private ConnectionType _connectionType;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string connectionString = "", ConnectionType connectionType = ConnectionType.MSSQL) : base(options) 
        {
            _connectionString = connectionString;
            _connectionType = connectionType;
        }

		#region Tables

		public DbSet<BroadcastInfo> Broadcasts { get; set; }

        public DbSet<BroadcastInfoKeyword> BroadcastsKeywords { get; set; }

        public DbSet<Tag> Tags { get; set; }

		public DbSet<BroadcastCategory> Categories { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }


        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //// move to config
            //int titeLength = 250;
            //modelBuilder.Entity<BroadcastInfo>().Property(x => x.Title).IsRequired().HasMaxLength(titeLength);

            builder.Entity<BroadcastInfo>()
                .Property(x => x.IndexingStatus)
                .HasDefaultValue(1);

            builder.Entity<BroadcastCategory>()
                .Property(x => x.IndexingStatus)
                .HasDefaultValue(1);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!String.IsNullOrEmpty(_connectionString))
            {
                ConfigServer(_connectionString, optionsBuilder);
            }
        }

        public void ConfigServer(string connectionString, DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionType == ConnectionType.MSSQL)
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");
                optionsBuilder.UseSqlServer(_connectionString);
            }
            else if (_connectionType == ConnectionType.Postgres)
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }
    }
}