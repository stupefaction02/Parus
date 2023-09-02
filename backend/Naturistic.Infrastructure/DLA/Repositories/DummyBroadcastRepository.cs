using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;

namespace Naturistic.Infrastructure.DLA
{
	public class DummyBroadcastRepository : IBroadcastRepository
	{
		private readonly ApplicationDbContext context;
	
		public DummyBroadcastRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public Task<IEnumerable<BroadcastInfo>> GetAllAsync()
		{
			throw new NotImplementedException();
		}
	}
}