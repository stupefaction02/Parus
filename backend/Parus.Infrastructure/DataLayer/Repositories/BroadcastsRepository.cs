﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DataLayer.Repositories
{
    public class BroadcastCategoryRepository : IBroadcastCategoryRepository
    {
        public IEnumerable<BroadcastCategory> Categories { get => context.Categories; }

        private readonly ParusDbContext context;

        public BroadcastCategoryRepository(ParusDbContext context)
        {
            this.context = context;
        }

        public void UpdateWithoutContextSave(BroadcastCategory cat)
        {
            context.Categories.Update(cat);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }

    public class BroadcastsRepository : IBroadcastInfoRepository
    {
        private readonly ParusDbContext context;

        public BroadcastsRepository(ParusDbContext context)
        {
            this.context = context;
        }

        public int Count()
        {
            return context.Broadcasts.Count();
        }

        public IEnumerable<Broadcast> GetInterval(int start, int count)
        {
            return context.Broadcasts
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Skip(start).Take(count);
        }

        public Broadcast One(Func<Broadcast, bool> predicate)
        {
            return context.Broadcasts.Include(x => x.Category)
                .Include(x => x.Tags)
                .AsEnumerable().SingleOrDefault(predicate);
        }

        public Broadcast OneLazy(Func<Broadcast, bool> predicate)
        {
            return context.Broadcasts
                .AsEnumerable().SingleOrDefault(predicate);
        }

        /// <summary>
        /// Deletes with checking on existing
        /// </summary>
        /// <param name="userId"></param>
        public void RemoveOne(string userId)
        {
            Broadcast target = OneLazy(x => x.HostUserId == userId);

            if (target != null)
            {
                context.Broadcasts.Remove(target);

                context.SaveChanges();
            }
        }

        public void Add(Broadcast broadcast)
        {
            context.Add(broadcast);

            context.SaveChanges();
        }

        const string titleProp = nameof(BroadcastInfoKeyword.Keyword);

        public IEnumerable<Broadcast> Broadcasts { get => context.Broadcasts.Include(x => x.Category).Include(x => x.Tags); }

        //[Benchmark(Description = "BroadcastInfoRepository.Search")]
        public IEnumerable<Broadcast> Search(string query)
        {
            return context.BroadcastsKeywords
                .Include(x => x.BroadcastInfo)
                .ThenInclude(x => x.Category)
                .Include(x => x.BroadcastInfo)
                .ThenInclude(x => x.Tags)
                .Where(x => EF.Functions.Like(x.Keyword, $"%{query}%"))
                .Select(x => x.BroadcastInfo);
        }

        public void UpdateWithoutContextSave(Broadcast user)
        {
            context.Broadcasts.Update(user);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }
    }
}
