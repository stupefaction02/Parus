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
    //public class BroadcastInfoToJson
    //{
    //    [JsonPropertyName("title")]
    //    public string Title { get; set; }

    //    [JsonPropertyName("hostUsername")]
    //    public string HostUsername { get; set; }

    //    [JsonPropertyName("ava")]
    //    public string Ava { get; set; }

    //    [JsonPropertyName("ref")]
    //    public int Reference { get; set; }
    //}

    public class BroadcastsCategoryIndexer : Indexer
    {
        private static string _indexPrefix = "bcats_";
        private readonly IBroadcastCategoryRepository repository;
        private FirstLetterIndexingRule _indexingRule = new FirstLetterIndexingRule(_indexPrefix);

        public BroadcastsCategoryIndexer(IBroadcastCategoryRepository repository)
        {
            this.repository = repository;
        }

        public override async Task RunIndexing()
        {
            int changed = 0;
            int total = repository.Categories.Count();

            foreach (BroadcastCategory broadcast in repository.Categories)
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

            await repository.SaveChangesAsync();

            Console.WriteLine($"BroadcastsCategoryIndexer has done its work. Changed entries: {changed}, Total: {total}");
        }

        private async Task<bool> ProcessAddToIndexStatus(BroadcastCategory category,
            IBroadcastCategoryRepository repository)
        {
            string title = category.Name;

            string indexName = _indexingRule.Index(title);

            await AddIndex(indexName);

            string json = JsonSerializer.Serialize( category );

            bool added = await PutToIndex(indexName, category.Id + "_", json);

            if (added)
            {
                Console.WriteLine($"Added {title} to index {indexName}.");

                category.IndexingStatus = (byte)IndexingRule.DoNothing;

                repository.UpdateWithoutContextSave(category);

                return true;
            }

            return false;
        }
    }
}
