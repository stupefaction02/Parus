using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Infrastructure.DLA;
using Naturistic.Infrastructure.Identity;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Naturistic.WebUI.Services
{
    public class MSSQLSearchingService : ISearchingService
    {
        private readonly IBroadcastInfoRepository broadcasts;
        private readonly ApplicationIdentityDbContext usersdentityCtx;
        private readonly ApplicationDbContext data;

        public MSSQLSearchingService(IBroadcastInfoRepository broadcasts, 
            ApplicationIdentityDbContext usersdentityCtx,
            ApplicationDbContext data)
        {
            this.broadcasts = broadcasts;
            this.usersdentityCtx = usersdentityCtx;
            this.data = data;
        }

        public IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q, int count)
        {
            return broadcasts.Search(q).Take(count);
        }

        public IEnumerable<Tag> SearchTagsByName(string q, int count)
        {
            return data.Tags
                .OrderBy(x => x.Name)
                .Where(x => EF.Functions.Like(x.Name, $"%{q}%"))
                .Take(count)
                .ToList();
        }

        public IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count)
        {
            return data.Categories
                .OrderBy(x => x.Name)
                .Where(x => EF.Functions.Like(x.Name, $"%{q}%"))
                .Take(count)
                .ToList();
        }

        public IEnumerable<IUser> SearchUsersByName(string q, int v)
        {
            //return users.Users.Where(x => EF.Functions.Like(x.GetUsername(), $"%{q}%"))
            //    .Take(v);

            return usersdentityCtx.Users
                .OrderBy(x => x.UserName)
                .Where(x => EF.Functions.Like(x.UserName, $"%{q}%"))
                .Take(v)
                .ToList();
        }
    }
}
