using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace Parus.Core.Services.ElasticSearch
{
    public class UsersIndexer : Indexer
    {
        private readonly Func<IUserRepository> getUsersAction;
        private readonly ElasticTransport transport;

        private readonly string indexPrefix = "users_";
        private string usersIndexSettings_2pshrad_1replica = "{\"settings\":{\"number_of_shards\": 2,\"number_of_replicas\": 1},\"mappings\":{\"properties\":{\"username\":{\"type\": \"keyword\",\"index\": \"true\"},\"ava\": { \"type\": \"keyword\",\"index\": \"false\"},\"description\": {\"type\":\"text\",\"index\": \"false\"}, \"subsCount\":{\"type\":\"long\",\"index\":\"false\"}}}}";

        public UsersIndexer(Func<IUserRepository> getUsers, string cdnUrl, ElasticTransport transport) : base(transport)
        {
            this.cdnAvasFolderPath = cdnUrl + "/";
            this.getUsersAction = getUsers;
            this.transport = transport;
        }

        public async Task RunIndexing()
        {
            var users = getUsersAction?.Invoke();

            List<string> addedIndexes = await GetIndexesAsync() ?? new List<string>();

            if (users == null)
            {
                throw new ArgumentException("Couldn't find service {}");
            }

            int changed = 0;
            int total = users.Users.Count();

            foreach (IUser user in users.Users)
            {
                switch (user.GetIndexingRule())
                {
                    case 0:

                        break;
                    case 1:
                        string username = user.GetUsername();
                        char firstLetter = username[0];
                        // this char defines and index
                        byte c = (byte)firstLetter;
                        string indexName = "";
                        if (c >= 128)
                        {
                            // Non Ascii
                            indexName = indexPrefix + "na";
                        }
                        else
                        {
                            indexName = indexPrefix + Char.ToString(firstLetter).ToLower();
                        }

                        bool indexExists = addedIndexes.Contains(indexName);

                        if (!indexExists)
                        {
                            //Console.WriteLine(picturesIndexSettings);
                            bool created = await AddIndexAsync(indexName, this.usersIndexSettings_2pshrad_1replica);

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
                        else
                        {
                            //Console.WriteLine($"Index {indexName} is already in engine.");
                        }

                        bool added = await PutUserToIndexAsync(indexName,
                                            user.GetId() + "_",
                                            username,
                                            cdnAvasFolderPath + user.GetAvatarPath(),
                                            $"Hello, I'm {username}. Streamer and youtuber.",
                                            168);
                        if (added)
                        {
                            Console.WriteLine($"Added {username} to index {indexName}.");

                            user.SetIndexingRule(IndexingRule.DoNothing);

                            users.UpdateWithoutContextSave(user);

                            Console.CursorLeft = 0;
                            changed++;
                        }

                        break;
                }
            }

            users.SaveChanges();

            Console.WriteLine($"Done! {changed}/{total}");
        }

        // pull from configs
        private readonly string username = "username";
        private readonly string avapath = "avapath";
        private readonly string description = "description";
        private readonly string subCountsStr = "subCountsStr";

        private readonly string _doc_p = "/_doc/";
        private readonly string cdnAvasFolderPath;

        protected async Task<bool> PutUserToIndexAsync(string indexName, string id,
            string username, string avapath, string description, int subscirbersCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('{');

            sb.Append($"\"{this.username}\":\"{username}\",");
            sb.Append($"\"{this.avapath}\":\"{avapath}\",");
            sb.Append($"\"{this.description}\":\"{description}\",");
            sb.Append($"\"{this.subCountsStr}\":\"{subscirbersCount}\"");

            sb.Append('}');

            var response = await this.transport.PutStringAsync($"{indexName}{this._doc_p}{id}", sb.ToString());
            if ((response == HttpStatusCode.Created) || (response == HttpStatusCode.OK))
            {
                return true;
            }

            return false;
        }
    }
}
