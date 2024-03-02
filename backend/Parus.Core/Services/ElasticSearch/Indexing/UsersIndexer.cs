using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Parus.Core.Services.ElasticSearch.Indexing
{
    public class UserElasticDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("avapath")]
        public string Avapath { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("subCountsStr")]
        public int SubCountsStr { get; set; }
    }

    public class UsersIndexer : Indexer
    {
        private readonly string _cdnAvasFolderPath;
        private readonly ElasticTransport _transport;
        private readonly IUserRepository repository;
        private static string _indexPrefix = "users_";
        private FirstLetterIndexingRule _indexingRule = new FirstLetterIndexingRule(_indexPrefix);

        public UsersIndexer(string cdnUrl, IUserRepository users) : base()
        {
            _cdnAvasFolderPath = cdnUrl + "/";
            this.repository = users;
        }

        public override async Task RunIndexing()
        {
            if (repository == null)
            {
                throw new ArgumentException("Couldn't find service {}");
            }

            int succesed = 0;
            int total = repository.Users.Count();

            try
            {
                foreach (IUser user in repository.Users)
                {
                    switch (user.GetIndexingStatus())
                    {
                        case 0:

                            break;
                        case 1:
                            if (await ProcessAddToIndexStatus(user, repository))
                            {
                                succesed++;
                            }

                            break;
                    }
                }

                repository.SaveChanges();
            }
            catch (Exception ex)
            {
                // catch network exception
            }

            Console.WriteLine($"UsersIndexer has done its work. Changed entris: {succesed}, Total: {total}");
        }

        private async Task<bool> ProcessAddToIndexStatus(IUser user, IUserRepository repository)
        {
            string username = user.GetUsername();

            string indexName = _indexingRule.Index(username);

            await AddIndex(indexName);

            UserElasticDto userData = new UserElasticDto
            {
                Username = user.GetUsername(),
                Description = "aaaa",
                Avapath = user.GetAvatarPath(),
                SubCountsStr = 164
            };

            string userDataString = JsonSerializer.Serialize(userData);

            bool added = await PutToIndex(indexName, user.GetId() + "_", userDataString);

            if (added)
            {
                Console.WriteLine($"Added {username} to index {indexName}.");

                user.SetIndexingRule(IndexingRule.DoNothing);

                repository.UpdateWithoutContextSave(user);

                return true;
            }

            return false;
        }
    }
}
