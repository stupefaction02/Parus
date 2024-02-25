using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Parus.Common.Extensions;
using System.Linq;
using System.Text;

namespace Parus.Core.Services.ElasticSearch
{
    public class ElasticSearchEngine : IDisposable
    {
        private Thread indexingThread;
        private readonly IUserRepository users;
        private readonly ElasticTransport transport;

        // To get:
        // 1. bin/elastic-reset-password for user elastic
        // 2. encode elatic:<password from above step> with Base64
        // 3. Done!
        private string password => "ZWxhc3RpYzpHUUp5YzI0U3IxdFBOdDRPc25Ccw==";
        private string host => "http://localhost:9200";
        private readonly string indexPrefix = "users_";
        private string usersIndexSettings_2pshrad_1replica = "{\"settings\":{\"number_of_shards\": 2,\"number_of_replicas\": 1},\"mappings\":{\"properties\":{\"username\":{\"type\": \"keyword\",\"index\": \"true\"},\"ava\": { \"type\": \"keyword\",\"index\": \"false\"},\"description\": {\"type\":\"text\",\"index\": \"false\"}, \"subsCount\":{\"type\":\"long\",\"index\":\"false\"}}}}";

        public ElasticSearchEngine(string cdnUrl)//IUserRepository users)
        {
            this.indexingThread = new Thread(RunIndexing);
            //this.users = users;

            var transport = new ElasticTransport(host, auth: ("Basic", password));

            try
            {
                transport.Connect();
            }
            catch (Exception ex)
            {

            }

            this.indexingThread.Start();

            this. transport = transport;
            this.cdnAvasFolderPath = cdnUrl + "/";
        }

        public Func<IUserRepository> GetUsersAction;

        public void RunIndexing()
        {
            var indexingUsersTask = IndexUsersAsync();

            

            while (true)
            {

                // update configs
                Thread.Sleep(1000);
            }
        }

        private async Task IndexUsersAsync()
        {
            var users = GetUsersAction?.Invoke();

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

                            user.SetIndexingRule( IndexingRule.DoNothing );

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

        private struct ElasticUserModel
        {
            public string Username;
            public string Avapath;
            public string Description;
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

        public struct Setting
        {
            public string Name;
            public object Value;
        }

        private async Task<List<string>> GetIndexesAsync()
        {
            List<string> names = new List<string>();

            var response = await this.transport.GetContentAsync("_cat/indices");

            if (response.Item2 == HttpStatusCode.OK)
            {
                using (StreamReader ms = new StreamReader(response.Item1))
                {
                    string line;
                    while ((line = ms.ReadLine()) != null)
                    {
                        names.Add( line.GetStringInSpacedString(2) );
                    }
                }

                return names;
            }

            return null;
        }

        public async Task<bool> AddIndexAsync(string name, string setings)
        {
            return await this.transport.PutStringAsync(name, setings) == HttpStatusCode.OK;
        }

        public void Dispose()
        {
            indexingThread.Abort();

            indexingThread = null;
        }
    }
}
