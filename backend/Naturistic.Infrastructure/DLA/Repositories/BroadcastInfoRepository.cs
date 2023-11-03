using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DLA.Repositories
{
    public class BroadcastInfoRepository : IBroadcastInfoRepository
    {
        private readonly ApplicationDbContext context;

        public BroadcastInfoRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public int Count()
        {
            return this.context.Broadcasts.Count();
        }

        public IEnumerable<BroadcastInfo> GetInterval(int start, int count)
        {
            return this.context.Broadcasts
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Skip(start).Take(count);
        }

        public BroadcastInfo One(Func<BroadcastInfo, bool> predicate)
        {
            return context.Broadcasts.Include(x => x.Category)
                .Include(x => x.Tags)
                .AsEnumerable().SingleOrDefault(predicate);
        }

        public BroadcastInfo OneLazy(Func<BroadcastInfo, bool> predicate)
        {
            return context.Broadcasts
                .AsEnumerable().SingleOrDefault(predicate);
        }

        public void RemoveOne(string userId)
        {
            BroadcastInfo target = OneLazy(x => x.HostUserId == userId);

            context.Broadcasts.Remove(target);

            context.SaveChanges();
        }

        public void Add(BroadcastInfo broadcast)
        {
            context.Add(broadcast);

            context.SaveChanges();
        }

        const string titleProp = nameof(BroadcastInfoKeyword.Keyword);

        //[Benchmark(Description = "BroadcastInfoRepository.Search")]
        public IEnumerable<BroadcastInfo> Search(string query)
        {
            return context.BroadcastsKeywords
                .Include(x => x.BroadcastInfo)
                .ThenInclude(x => x.Category)
                .Include(x => x.BroadcastInfo)
                .ThenInclude(x => x.Tags)
                .Where(x => EF.Functions.Like(x.Keyword, $"%{query}%"))
                .Select(x => x.BroadcastInfo);
        }
    }
}
