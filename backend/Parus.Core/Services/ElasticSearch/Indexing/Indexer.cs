using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;
using Parus.Common.Extensions;

namespace Parus.Core.Services.ElasticSearch.Indexing
{

    public class FirstLetterIndexingRule
    {
        private readonly string _indexPrefix;

        public FirstLetterIndexingRule(string indexPrefix)
        {
            _indexPrefix = indexPrefix;
        }

        public string Index(string str)
        {
            char firstLetter = str[0];
            // this char defines and index
            byte c = (byte)firstLetter;
            string indexName = "";
            if (c >= 128)
            {
                // Non Ascii
                indexName = _indexPrefix + "na";
            }
            else
            {
                indexName = _indexPrefix + char.ToString(firstLetter).ToLower();
            }

            return indexName;
        }
    }

    public abstract class Indexer
    {
        private ElasticTransport _transport;
        private readonly string _doc_p = "/_doc/";

        public Indexer()
        {
            
        }

        public Indexer(ElasticTransport transport)
        {
            this._transport = transport;
        }

        public async Task<bool> AddIndexAsync(string name, string setings)
        {
            return await _transport.PutStringAsync(name, setings) == HttpStatusCode.OK;
        }

        protected async Task<bool> PutToIndex(string indexName, string id, string userDataString)
        {
            var response = await _transport.PutStringAsync($"{indexName}{_doc_p}{id}", userDataString);
            if (response == HttpStatusCode.Created || response == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        protected async Task<bool> PostToIndex(string indexName, string id, string userDataString)
        {
            var response = await _transport.PostStringAsync($"{indexName}{_doc_p}{id}", userDataString);
            if (response == HttpStatusCode.Created || response == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        List<string> _indexes;

        protected async Task<List<string>> GetIndexesAsync()
        {
            if (_indexes != null)
            {
                return _indexes;
            }

            List<string> names = new List<string>();

            var response = await _transport.GetContentAsync("_cat/indices");

            if (response.Item2 == HttpStatusCode.OK)
            {
                using (StreamReader ms = new StreamReader(response.Item1))
                {
                    string line;
                    while ((line = ms.ReadLine()) != null)
                    {
                        names.Add(line.GetStringInSpacedString(2));
                    }
                }

                return _indexes = names;
            }

            return null;
        }

        private string usersIndexSettings_2pshrad_1replica = "{\"settings\":{\"number_of_shards\": 2,\"number_of_replicas\": 1},\"mappings\":{\"properties\":{\"username\":{\"type\": \"keyword\",\"index\": \"true\"},\"ava\": { \"type\": \"keyword\",\"index\": \"false\"},\"description\": {\"type\":\"text\",\"index\": \"false\"}, \"subsCount\":{\"type\":\"long\",\"index\":\"false\"}}}}";

        public ElasticTransport Transport { get => _transport; set => _transport = value; }

        protected async Task AddIndex(string indexName)
        {
            List<string> addedIndexes = await GetIndexesAsync() ?? new List<string>();

            bool indexExists = addedIndexes.Contains(indexName);

            if (!indexExists)
            {
                bool created = await AddIndexAsync(indexName, usersIndexSettings_2pshrad_1replica);

                if (created)
                {
                    addedIndexes.Add(indexName);

                    Console.WriteLine($"Index {indexName} was created.");
                }
                else
                {
                    // TODO: this.engine.LastIndexError
                    Console.WriteLine($"Couldn't create index {indexName}.");
                }
            }
        }

        public virtual async Task RunIndexing() { await Task.CompletedTask; }
    }
}
