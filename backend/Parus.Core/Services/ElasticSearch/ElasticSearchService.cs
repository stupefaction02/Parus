using Parus.Core.Entities;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services.ElasticSearch.Indexing;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parus.Core.Services.ElasticSearch
{
    public partial class ElasticSearchService
    {
        public class SearchLiteResult<T>
        {
            [JsonPropertyName("hits")]
            public SearchResultHits<T> Hits { get; set; }
        }

        public class SearchResultHits<T>
        {
            [JsonPropertyName("total")]
            public SearchLiteResultHitsTotal Total { get; set; }

            [JsonPropertyName("hits")]
            public IEnumerable<SearchResultHit<T>> Hits { get; set; }
        }

        public class SearchResultHit<T>
        {
            //BroadcastInfoElasticDto
            [JsonPropertyName("_source")]
            public T Source { get; set; }
        }

        public class SearchLiteResultHitsTotal
        {
            [JsonPropertyName("value")]
            public int Value { get; set; }
        }
    }

    public struct Result 
    {
        public object Items;
        public int TotalCount;

        public Result(int totalCount, object items) : this()
        {
            TotalCount = totalCount;
            Items = items;
        }
    }

    public partial class ElasticSearchService
    {
        private static string searchUsernamePath = "_search?q=username:";
        private static string searchTitlePath = "_search?q=title:";
        private static string searchCatNamePath = "_search?q=name:";

        private readonly string elasticHost;
        private readonly ElasticTransport _transport;

        public ElasticSearchService(ElasticTransport transport)
        {
            this._transport = transport;
        }

        public async Task<Result> LiteSearch<T>(string url)
        {
            (HttpStatusCode, string) result = await _transport.GetStringAsync(url);

            if (result.Item1 == HttpStatusCode.OK)
            {
                var r = JsonSerializer.Deserialize
                    <SearchLiteResult<T>>(result.Item2);

                return new Result(r.Hits.Total.Value, r.Hits.Hits.Select(x => x.Source));
            }

            return new Result();
        }

        public async Task<Result> SearchBroadcastsByTitleTagsAsync(string query, int start, int count)
        {
            // TODO: Caching :<
            string url = searchTitlePath + query + $"&size={count}&from={start}";

            return await LiteSearch<BroadcastInfoElasticDto>(url);
        }

        public async Task<Result> SearchUsersByUsernameAsync(string query, int start, int count)
        {
            string url = searchUsernamePath + query + $"&size={count}&from={start}";

            return await LiteSearch<UserElasticDto>(url);
        }

        public async Task<Result> SearchCategoriesByNameAsync(string query, int start, int count)
        {
            string url = searchCatNamePath + query + $"&size={count}&from={start}";

            return await LiteSearch<BroadcastCategory>(url);
        }
    }
}
