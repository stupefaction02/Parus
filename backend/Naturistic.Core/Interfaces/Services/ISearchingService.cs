using System;
using System.Collections.Generic;
using System.Text;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Services
{
    public interface ISearchingService
    {
        IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q, int count);
        IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count);
        IEnumerable<Tag> SearchTagsByName(string q, int count);
        IEnumerable<IUser> SearchUsersByName(string q, int v);
    }
}
