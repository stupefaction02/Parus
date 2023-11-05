using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parus.Core.Entities;

namespace Parus.Core.Interfaces.Services
{
    public interface ISearchingService
    {
        IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q, int count);
        IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count);
        IEnumerable<Tag> SearchTagsByName(string q, int count);
        IEnumerable<IUser> SearchUsersByName(string q, int v);
        IQueryable<IUser> SearchUsersByName(string q);
    }
}
