using System;
using System.Diagnostics;
using Cassandra;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Naturistic.Infrastructure.Identity
{
    public class CassandraDbIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public CassandraDbIdentityContext(ILogger<CassandraDbIdentityContext> logger,
            DbContextOptions options) : base(options)
        {
            logger.LogInformation("Configuring Cassandra Database ...");
            Debug.WriteLine("Configuring Cassandra Database ...");

            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();

                var session = cluster.Connect();

                if (session != null)
                {
                    Debug.WriteLine("Connected successfully!");
                    logger.LogInformation("Connected successfully!");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Connection Error!");
                logger.LogError(e.Message);
            }
        }
    }
}