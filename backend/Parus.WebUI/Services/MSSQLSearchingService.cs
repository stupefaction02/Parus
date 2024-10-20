using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Identity;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Parus.WebUI.Services
{
    public class MSSQLSearchingService //: ISearchingService
    {
        private readonly IBroadcastInfoRepository broadcasts;
        private readonly ParusDbContext usersdentityCtx;
        private readonly ApplicationDbContext data;

        public MSSQLSearchingService(IBroadcastInfoRepository broadcasts, 
            ParusDbContext usersdentityCtx,
            ApplicationDbContext data)
        {
            this.broadcasts = broadcasts;
            this.usersdentityCtx = usersdentityCtx;
            this.data = data;
        }

        public IEnumerable<Broadcast> SearchBroadcastsByTitleTags(string query, int start, int count)
        {
            return null;
        }

        public IEnumerable<Broadcast> SearchBroadcastsByTitleTags(string q, int count)
        {
            return broadcasts.Search(q).Take(count);
        }

        public IEnumerable<Broadcast> SearchBroadcastsByTitleTags(string q)
        {
            return broadcasts.Search(q);
        }

        public IEnumerable<BroadcastTag> SearchTagsByName(string q, int count)
        {
            return data.Tags
                .OrderBy(x => x.Name)
                .Where(x => EF.Functions.Like(x.Name, $"%{q}%"))
                .Take(count)
                .ToList();
        }

        public int CountBroadcastsByTitleTags(string query)
        {
            return data.BroadcastsKeywords
                .Count(x => EF.Functions.Like(x.Keyword, $"%{query}%"));
        }



        public IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count)
        {
            return data.Categories
                .OrderBy(x => x.Name)
                .Where(x => EF.Functions.Like(x.Name, $"%{q}%"))
                .Take(count)
                .ToList();
        }

        public IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int start, int count)
        {
            return data.Categories
                .OrderBy(x => x.Name)
                .Where(x => EF.Functions.Like(x.Name, $"%{q}%"))
                .Skip(start)
                .Take(count)
                .ToList();
        }

        public int CountCategoriesByName(string query)
        {
            return data.Categories
                .OrderBy(x => x.Name)
                .Count(x => EF.Functions.Like(x.Name, $"%{query}%"));
        }

        public IEnumerable<IUser> SearchUsersByName(string query, int count)
        {
            return usersdentityCtx.Users
                .OrderBy(x => x.UserName)
                .Where(x => EF.Functions.Like(x.UserName, $"%{query}%"))
                .Take(count)
                .ToList();
        }


        public IQueryable<IUser> SearchUsersByName(string query)
        {
            return usersdentityCtx.Users
                .OrderBy(x => x.UserName)
                .Where(x => EF.Functions.Like(x.UserName, $"%{query}%"));
        }

        public int CountUsersByName(string query)
        {
            return usersdentityCtx.Users
                .OrderBy(x => x.UserName)
                .Count(x => EF.Functions.Like(x.UserName, $"%{query}%"));
        }

        public IEnumerable<BroadcastCategory> SearchCategoryByName(string query)
        {
            return data.Categories
                .OrderBy(x => x.Name)
                .Where(x => EF.Functions.Like(x.Name, $"%{query}%"));
        }

        public IEnumerable<IUser> SearchUsersByName(string query, int start, int count)
        {
            return usersdentityCtx.Users
                .OrderBy(x => x.UserName)
                .Where(x => EF.Functions.Like(x.UserName, $"%{query}%"))
                .Skip(start)
                .Take(count)
                .ToList();
        }
    }
}
