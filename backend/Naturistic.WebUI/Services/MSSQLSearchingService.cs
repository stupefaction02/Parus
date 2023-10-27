using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Infrastructure.DLA;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Naturistic.WebUI.Services
{
    public class MSSQLSearchingService : ISearchingService
    {
        private readonly IBroadcastInfoRepository broadcasts;
        private readonly IUserRepository users;
        private readonly ApplicationDbContext data;

        public MSSQLSearchingService(IBroadcastInfoRepository broadcasts, 
            IUserRepository users,
            ApplicationDbContext data)
        {
            this.broadcasts = broadcasts;
            this.users = users;
            this.data = data;
        }

        public IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q, int count)
        {
            return broadcasts.Search(q).Take(count);
        }

        public IEnumerable<Tag> SearchTagsByName(string q, int count)
        {
            return data.Tags.Where(x => x.Name == q);
        }

        public IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count)
        {
            return data.Categories;
        }
    }
}
