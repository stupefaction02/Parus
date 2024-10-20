using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;

namespace Parus.Core.Services.ElasticSearch.Indexing
{
    public class BroadcastsIndexer : Indexer
    {
        private static string _indexPrefix = "broadcasts";
        private readonly IBroadcastInfoRepository repository;
        private CustomNameIndexingRule _indexingRule = new CustomNameIndexingRule(_indexPrefix);

        public BroadcastsIndexer(IBroadcastInfoRepository repository)
        {
            this.repository = repository;
        }

        public override async Task RunIndexing()
        {
            int changed = 0;
            int total = repository.Broadcasts.Count();

            foreach (Broadcast broadcast in repository.Broadcasts)
            {
                switch (broadcast.IndexingStatus)
                {
                    case 0:

                        break;
                    case 1:
                        if (await ProcessAddToIndexStatus(broadcast, repository))
                        {
                            changed++;
                        }

                        break;
                }
            }

            repository.SaveChanges();

            Console.WriteLine($"BroadcastsIndexer has done its work. Changed entries: {changed}, Total: {total}");
        }

        private async Task<bool> ProcessAddToIndexStatus(Broadcast broadcast, IBroadcastInfoRepository repository)
        {
            string title = broadcast.Title;

            string indexName = _indexingRule.Index(title);

            await AddIndex(indexName);

            string json = JsonSerializer.Serialize( broadcast.ElasticDto() );

            bool added = await PutToIndex(indexName, broadcast.Id + "_", json);

            if (added)
            {
                Console.WriteLine($"Added {title} to index {indexName}.");

                broadcast.IndexingStatus = (byte)IndexingRule.DoNothing;

                repository.UpdateWithoutContextSave(broadcast);

                return true;
            }

            return false;
        }
    }
}
