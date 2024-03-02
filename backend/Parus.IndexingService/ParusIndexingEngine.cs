using Microsoft.EntityFrameworkCore;
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
        private T ConfigureContext<T>() where T : DbContext
        {
            Type type = typeof(T);
            string key = type.Name;
            if (!store.TryGetValue(key, out DbContext ctx))
            {
                var optionsBuilder = new DbContextOptionsBuilder<T>();
                var options = optionsBuilder.Options;

                ctx = (T)Activator.CreateInstance(type, options);
                store.Add(key, ctx);
            }

            return (T)ctx;
        }

        private protected IUserRepository users;
        protected IUserRepository Users
        {
            get
            {
                if (users != null) return users;

                var ctx = ConfigureContext<ApplicationIdentityDbContext>();
                return users = new UserRepository(ctx);
            }
        }

        private protected IBroadcastInfoRepository broadcasts;
        protected IBroadcastInfoRepository Broadcasts
        {
            get
            {
                if (broadcasts != null) return broadcasts;

                var ctx = ConfigureContext<ApplicationDbContext>();
                return broadcasts = new BroadcastInfoRepository(ctx);
            }
        }

        private protected IBroadcastCategoryRepository categories;
        protected IBroadcastCategoryRepository Categories
        {
            get
            {
                if (categories != null) return categories;

                var ctx = ConfigureContext<ApplicationDbContext>();
                return categories = new BroadcastCategoryRepository(ctx);
            }
        }

        public ParusIndexingEngine(string cdnUrl) : base(cdnUrl)
        {
            IndexingQueue.Add(new UsersIndexer(cdnUrl, Users));
            IndexingQueue.Add(new BroadcastsIndexer(Broadcasts));
            IndexingQueue.Add(new BroadcastsCategoryIndexer(Categories));
        }
    }
}
