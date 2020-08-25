using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
	public interface IBroadcastRepository
	{
		Task<IEnumerable<BroadcastInfo>> GetAllAsync();
	}
}