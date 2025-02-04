﻿using Parus.Core.Entities;
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
using System.Collections;

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
            public List<SearchResultHit<T>> Hits { get; set; }
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

    public class SearchResult
    {
        // json, text
        public string Raw { get; set; } = "";
        public int TotalCount { get; set; }
    }

    public class BroadcastsSearchResult : SearchResult
    {
        public IEnumerable<BroadcastInfoElasticDto> Items { get; set; }
    }

    public class UsersSearchResult : SearchResult
    {
        public IEnumerable<UserElasticDto> Items { get; set; }
    }

    public class BroadcastCategorySearchResult : SearchResult
    {
        public IEnumerable<BroadcastCategory> Items { get; set; }
    }

    public partial class ElasticSearchService : ISearchingService
    {
        private static string searchUsernamePath = "_search?q=username:";
        private static string searchTitlePath = "_search?q=title:";
        private static string searchCatNamePath = "categories/_search?q=name:";

        private readonly ElasticTransport _transport;

        public ElasticSearchService(ElasticTransport transport)
        {
            this._transport = transport;
        }

        public async Task<SearchLiteResult<T>> LiteSearch<T>(string url)
        {
            (HttpStatusCode, string) result = await _transport.GetStringAsync(url);

            if (result.Item1 == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<SearchLiteResult<T>>(result.Item2);
            }

            return new SearchLiteResult<T>();
        }

        // ElasticSearch json to out json
        private string JsonToDtoJson(string json)
        {
            return JsonUtils.FormatElasticSearchResultJson(json);
        }

        public async Task<SearchResult> LiteSearchJson(string url)
        {
            (HttpStatusCode, string) result = await _transport.GetStringAsync(url);

            if (result.Item1 == HttpStatusCode.OK)
            {            
                return new SearchResult { Raw = JsonToDtoJson(result.Item2) };
            }
             
            return new SearchResult();
        }

        public async Task<BroadcastsSearchResult> SearchBroadcastsByTitleTagsAsync(string query, int start, int count)
        {
            // TODO: Caching :<
            string url = searchTitlePath + query + $"&size={count}&from={start}";

            var raw = (await LiteSearch<BroadcastInfoElasticDto>(url));

            return new BroadcastsSearchResult
            {
                Items = raw.Hits.Hits.Select(x => x.Source)
            };
        }

        public async Task<UsersSearchResult> SearchUsersByUsernameAsync(string query, int start, int count)
        {
            string url = searchUsernamePath + query + $"&size={count}&from={start}";

            var raw = (await LiteSearch<UserElasticDto>(url));

            return new UsersSearchResult
            {
                Items = raw.Hits.Hits.Select(x => x.Source)
            };
        }

        public async Task<BroadcastCategorySearchResult> SearchCategoriesByNameAsync(string query, int start, int count)
        {
            string url = searchCatNamePath + query + $"&size={count}&from={start}";

            var raw = (await LiteSearch<BroadcastCategory>(url));

            return new BroadcastCategorySearchResult
            {
                Items = raw.Hits.Hits.Select(x => x.Source)
            };
        }


        public async Task<SearchResult> SearchBroadcastsByTitleTagsJsonAsync(string query, int start, int count)
        {
            // TODO: Caching :<
            string url = searchTitlePath + query + $"&size={count}&from={start}";

            return await LiteSearchJson(url);
        }

        public async Task<SearchResult> SearchUsersByUsernameJsonAsync(string query, int start, int count)
        {
            string url = searchUsernamePath + query + $"&size={count}&from={start}";

            return await LiteSearchJson(url);
        }

        public async Task<SearchResult> SearchCategoriesByNameJsonAsync(string query, int start, int count)
        {
            string url = searchCatNamePath + query + $"&size={count}&from={start}";

            return await LiteSearchJson(url);
        }
    }
}
