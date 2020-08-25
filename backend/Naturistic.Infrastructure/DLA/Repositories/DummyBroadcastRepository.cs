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
	
		public async Task<IEnumerable<BroadcastInfo>> GetAllAsync()
		{
			return await Task.Run(() => Get());
		}
		
		public IEnumerable<BroadcastInfo> Get()
		{
			return new BroadcastInfo[5] { 
				new BroadcastInfo { BroadcastId = 1, Title = "Beatufiul place #1", Description = "Simple Broadcast#1" },
				new BroadcastInfo { BroadcastId = 2, Title = "Beatufiul place #2", Description = "Simple Broadcast#2" },
				new BroadcastInfo { BroadcastId = 3, Title = "Beatufiul place #3", Description = "Simple broadcast#3" },
				new BroadcastInfo { BroadcastId = 4, Title = "Beatufiul place #4", Description = "Simple broadcast#4" },
				new BroadcastInfo { BroadcastId = 5, Title = "Beatufiul place #5", Description = "Simple broadcast#5" }
			};
		}
	}
}