using Parus.Core.Entities;
using Parus.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;

namespace Parus.Core.Services.ElasticSearch
{
    public class ElasticSearchService : ISearchingService
    {
        private readonly ElasticSearchEngine engine;

        public ElasticSearchService(ElasticSearchEngine engine)
        {
            this.engine = engine;
        }

        public int CountBroadcastsByTitleTags(string query)
        {
            throw new System.NotImplementedException();
        }

        public int CountCategoriesByName(string query)
        {
            throw new System.NotImplementedException();
        }

        public int CountUsersByName(string q)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q, int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string q)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BroadcastInfo> SearchBroadcastsByTitleTags(string query, int start, int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BroadcastCategory> SearchCategoryByName(string q)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BroadcastCategory> SearchCategoryByName(string q, int start, int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Tag> SearchTagsByName(string q, int count)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<IUser> SearchUsersByName(string query)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IUser> SearchUsersByName(string query, int count)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IUser> SearchUsersByName(string query, int start, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}
