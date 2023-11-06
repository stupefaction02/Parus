using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parus.Core.Entities;

namespace Parus.Core.Interfaces.Services
{
    public interface ISearchingService
    {
        int CountBroadcastsByTitleTags(string query);
        int CountCategoriesByName(string query);
        int CountUsersByName(string q);
        IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q, int count);
        IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q);
        IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string query, int start, int count);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int start, int count);
        IEnumerable<Tag> SearchTagsByName(string q, int count);

        IQueryable<IUser> SearchUsersByName(string query);
        IEnumerable<IUser> SearchUsersByName(string query, int count);
        IEnumerable<IUser> SearchUsersByName(string query, int start, int count);

    }
}
