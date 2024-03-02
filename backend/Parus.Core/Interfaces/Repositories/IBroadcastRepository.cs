using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parus.Core.Entities;

namespace Parus.Core.Interfaces.Repositories
{
    public interface IBroadcastInfoRepository
    {
		int Count();

        IEnumerable<BroadcastInfo> Broadcasts { get; }

        IEnumerable<BroadcastInfo> GetInterval(int start, int count);
        BroadcastInfo One(Func<BroadcastInfo, bool> predicate);
        BroadcastInfo OneLazy(Func<BroadcastInfo, bool> predicate);
        void RemoveOne(string userId);
        IEnumerable<BroadcastInfo> Search(string query);
        void UpdateWithoutContextSave(BroadcastInfo user);
        int SaveChanges();
    }

    public interface IBroadcastCategoryRepository
    {
        IEnumerable<BroadcastCategory> Categories { get; }

        void UpdateWithoutContextSave(BroadcastCategory user);

        Task<int> SaveChangesAsync();
    }
}