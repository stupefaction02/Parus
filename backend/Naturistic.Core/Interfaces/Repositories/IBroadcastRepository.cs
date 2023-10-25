using System;
using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IBroadcastInfoRepository
    {
		int Count();

        IEnumerable<BroadcastInfo> GetInterval(int start, int count);
        BroadcastInfo One(Func<BroadcastInfo, bool> predicate);
        BroadcastInfo OneLazy(Func<BroadcastInfo, bool> predicate);
        void RemoveOne(string userId);
        IEnumerable<BroadcastInfo> Search(string query);
    }
}