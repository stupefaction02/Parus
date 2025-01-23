using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parus.Core.Entities;

namespace Parus.Core.Interfaces.Repositories
{
    public interface IBroadcastInfoRepository
    {
		int Count();

        IEnumerable<Broadcast> Broadcasts { get; }

        IEnumerable<Broadcast> GetInterval(int start, int count);
        Broadcast One(Func<Broadcast, bool> predicate);
        Broadcast OneLazy(Func<Broadcast, bool> predicate);
        void RemoveOne(string userId);
        IEnumerable<Broadcast> Search(string query);
        void UpdateWithoutContextSave(Broadcast user);
        int SaveChanges();
    }

    public interface IBroadcastCategoryRepository
    {
        IEnumerable<BroadcastCategory> Categories { get; }

        void UpdateWithoutContextSave(BroadcastCategory user);

        Task<int> SaveChangesAsync();
    }
}