using System.Threading.Tasks;
using Parus.Core.Services.ElasticSearch;

namespace Parus.Core.Interfaces.Services
{
    public interface ISearchingService
    {
        Task<BroadcastsSearchResult> SearchBroadcastsByTitleTagsAsync(string query, int start, int count);
        Task<BroadcastCategorySearchResult> SearchCategoriesByNameAsync(string query, int start, int count);
        Task<UsersSearchResult> SearchUsersByUsernameAsync(string query, int start, int count);
    }
}
