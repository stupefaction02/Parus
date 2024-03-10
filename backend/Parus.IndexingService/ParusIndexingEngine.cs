using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Services.ElasticSearch;
using Parus.Core.Services.ElasticSearch.Indexing;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Infrastructure.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Parus.IndexingService
{
    public class ParusIndexingEngine : ElasticIndexingEngine
    {
        Dictionary<string, DbContext> store = new Dictionary<string, DbContext>();
        private T ConfigureContext<T>(string connectionString) where T : DbContext
        {
            Type type = typeof(T);
            string key = type.Name;
            if (!store.TryGetValue(key, out DbContext? ctx))
            {
                var optionsBuilder = new DbContextOptionsBuilder<T>();
                var options = optionsBuilder.Options;

                ctx = (T)Activator.CreateInstance(type, options, connectionString);

                if (ctx != null)
                {
                    Console.WriteLine($"Created the instance of {type.Name}.");
                    store.Add(key, ctx);
                }
                else
                {
                    throw new Exception($"Cound't create instance of {type.Name}.");
                }
            }

            return (T)ctx;
        }

        private protected IUserRepository users;
        protected IUserRepository Users
        {
            get
            {
                if (users != null) return users;

                var ctx = ConfigureContext<ApplicationIdentityDbContext>(usersLogicConnecionString);
                return users = new UserRepository(ctx);
            }
        }

        private protected IBroadcastInfoRepository broadcasts;
        protected IBroadcastInfoRepository Broadcasts
        {
            get
            {
                if (broadcasts != null) return broadcasts;

                var ctx = ConfigureContext<ApplicationDbContext>(businessLogicConnecionString);
                return broadcasts = new BroadcastInfoRepository(ctx);
            }
        }

        private protected IBroadcastCategoryRepository categories;
        protected IBroadcastCategoryRepository Categories
        {
            get
            {
                if (categories != null) return categories;

                var ctx = ConfigureContext<ApplicationDbContext>(businessLogicConnecionString);

                return categories = new BroadcastCategoryRepository(ctx);
            }
        }

        string businessLogicConnecionString;
        string usersLogicConnecionString;
        public ParusIndexingEngine(IConfiguration configuration) : base(bulkMode: false)
        {
            businessLogicConnecionString = configuration["ConnectionStrings:BL:Default"];
            usersLogicConnecionString = configuration["ConnectionStrings:Users:Default"];

            string cdnUrl = configuration["Services:CDN:Main"];
            IndexingQueue.Add(new UsersIndexer(cdnUrl, Users));
            IndexingQueue.Add(new BroadcastsIndexer(Broadcasts));
            IndexingQueue.Add(new BroadcastsCategoryIndexer(Categories));
        }
    }
}
