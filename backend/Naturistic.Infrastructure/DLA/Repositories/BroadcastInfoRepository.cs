using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;

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
            return this.context.Broadcasts.Skip(start).Take(count);
        }
    }
}
