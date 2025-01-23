using Parus.Core.Interfaces.Services;
using System.Threading.Tasks;
using Parus.Core.Services.ElasticSearch;

namespace Parus.WebUI.Services
{
    public class DummySearchingService : ISearchingService
    {
        public Task<BroadcastsSearchResult> SearchBroadcastsByTitleTagsAsync(string query, int start, int count)
        {
            return Task.FromResult(new BroadcastsSearchResult());
        }

        public Task<BroadcastCategorySearchResult> SearchCategoriesByNameAsync(string query, int start, int count)
        {
            return Task.FromResult(new BroadcastCategorySearchResult());
        }

        public Task<UsersSearchResult> SearchUsersByUsernameAsync(string query, int start, int count)
        {
            return Task.FromResult(new UsersSearchResult());
        }
    }
}
