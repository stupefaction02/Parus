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
        IEnumerable<IBroadcastsInfoSearchResult> SearchBroadcastsByTitleTags(string q, int count);
        IEnumerable<IBroadcastsInfoSearchResult> SearchBroadcastsByTitleTags(string q);
        IEnumerable<IBroadcastsInfoSearchResult> SearchBroadcastsByTitleTags(string query, int start, int count);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int start, int count);
        IEnumerable<BroadcastTag> SearchTagsByName(string q, int count);

        IQueryable<IUserSearchResult> SearchUsersByName(string query);
        IEnumerable<IUserSearchResult> SearchUsersByName(string query, int count);
        IEnumerable<IUserSearchResult> SearchUsersByName(string query, int start, int count);

    }
}
