using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.DataLayer;

namespace Parus.Infrastructure.DataLayer.Repositories
{
    public class DummyBroadcastRepository// : IBroadcastRepository
    {
        private readonly ApplicationDbContext context;

        public DummyBroadcastRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task<IEnumerable<Broadcast>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}