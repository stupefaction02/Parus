using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Repositories
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
    }
}
