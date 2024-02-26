using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Parus.Common.Extensions;
using Parus.Core.Interfaces.Repositories;

namespace Parus.Core.Services.ElasticSearch
{
    public class BroadcastsIndexer : Indexer
    {
        private readonly Func<IBroadcastInfoRepository> getBroadcast;
        private Func<IBroadcastInfoRepository> getBroadcasts;

        public BroadcastsIndexer(Func<IBroadcastInfoRepository> getBroadcast, ElasticTransport transport
            ) : base(transport)
        {
            this.getBroadcast = getBroadcast;
        }

        public async Task RunIndexing()
        {
            var repository = this.getBroadcast?.Invoke();

            foreach (var item in repository.Broadcasts)
            {

            }
        }
    }

    public class Indexer
    {
        private readonly ElasticTransport transport;

        public Indexer(ElasticTransport transport)
        {
            this.transport = transport;
        }

        public async Task<bool> AddIndexAsync(string name, string setings)
        {
            return await this.transport.PutStringAsync(name, setings) == HttpStatusCode.OK;
        }

        protected async Task<List<string>> GetIndexesAsync()
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
                        names.Add(line.GetStringInSpacedString(2));
                    }
                }

                return names;
            }

            return null;
        }

    }
}
