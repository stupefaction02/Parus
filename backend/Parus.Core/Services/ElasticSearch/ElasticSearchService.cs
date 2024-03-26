using Parus.Core.Entities;
using Parus.Common.Utils;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services.ElasticSearch.Indexing;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Diagnostics;

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
        public string Json;
        public int TotalCount;

        public Result(string json) : this()
        {
            Json = json;
        }

        public Result(int totalCount, object items) : this(null)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }

    public partial class ElasticSearchService
    {
        private static string searchUsernamePath = "_search?q=username:";
        private static string searchTitlePath = "_search?q=title:";
        private static string searchCatNamePath = "categories/_search?q=name:";

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

                return new Result(r.Hits.Total.Value, r.Hits.Hits.Select(x => x.Source).ToList());
            }

            return new Result();
        }

        // ElasticSearch json to out json
        private string JsonToDtoJson(string json)
        {
            return JsonUtils.FormatElasticSearchResultJson(json);
        }

        public async Task<Result> LiteSearchJson(string url)
        {
            (HttpStatusCode, string) result = await _transport.GetStringAsync(url);

            if (result.Item1 == HttpStatusCode.OK)
            {            
                return new Result( JsonToDtoJson(result.Item2) );
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


        public async Task<Result> SearchBroadcastsByTitleTagsJsonAsync(string query, int start, int count)
        {
            // TODO: Caching :<
            string url = searchTitlePath + query + $"&size={count}&from={start}";

            return await LiteSearchJson(url);
        }

        public async Task<Result> SearchUsersByUsernameJsonAsync(string query, int start, int count)
        {
            string url = searchUsernamePath + query + $"&size={count}&from={start}";

            return await LiteSearchJson(url);
        }

        public async Task<Result> SearchCategoriesByNameJsonAsync(string query, int start, int count)
        {
            string url = searchCatNamePath + query + $"&size={count}&from={start}";

            return await LiteSearchJson(url);
        }
    }
}
