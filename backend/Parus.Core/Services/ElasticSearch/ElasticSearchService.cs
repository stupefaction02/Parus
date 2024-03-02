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
        public class SearchLiteResult
        {
            [JsonPropertyName("hits")]
            public SearchResultHits Hits { get; set; }
        }

        public class SearchResultHits
        {
            [JsonPropertyName("total")]
            public SearchLiteResultHitsTotal Total { get; set; }

            [JsonPropertyName("hits")]
            public IEnumerable<SearchResultHit> Hits { get; set; }
        }

        public class SearchResultHit
        {
            //BroadcastInfoElasticDto
            [JsonPropertyName("_source")]
            public BroadcastInfoElasticDto Source { get; set; }
        }

        public class SearchLiteResultHitsTotal
        {
            [JsonPropertyName("value")]
            public int Value { get; set; }
        }
    }

    public struct BroadcastResult 
    {
        IEnumerable<BroadcastInfoElasticDto> Items;
        public int TotalCount;

        public BroadcastResult(int totalCount, IEnumerable<BroadcastInfoElasticDto> items) : this()
        {
            TotalCount = totalCount;
            Items = items;
        }
    }

    public partial class ElasticSearchService
    {
        private static string searchPath = "_search?q=last_name:";
        private static string searchTitlePath = "_search?q=title:";
        private static string searchUserPath = "_search?q=username:";

        private readonly string elasticHost;
        private readonly ElasticTransport _transport;

        public ElasticSearchService(ElasticTransport transport)
        {
            this._transport = transport;
        }

        public async Task<BroadcastResult> SearchBroadcastsByTitleTagsAsync(string query, int start, int count)
        {
            // TODO: Caching :<
            string url = searchTitlePath + query + $"&size={count}&from={start}";

            (HttpStatusCode, string) result = await _transport.GetStringAsync(url);

            if (result.Item1 == HttpStatusCode.OK)
            {
                SearchLiteResult r = JsonSerializer.Deserialize<SearchLiteResult>(result.Item2);

                return new BroadcastResult(r.Hits.Total.Value,
                    r.Hits.Hits.Select(x => x.Source));
            }

            return new BroadcastResult();
        }
    }
}
