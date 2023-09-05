using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IBroadcastInfoRepository
    {
		int Count();

        IEnumerable<BroadcastInfo> GetInterval(int start, int count);
	}
}