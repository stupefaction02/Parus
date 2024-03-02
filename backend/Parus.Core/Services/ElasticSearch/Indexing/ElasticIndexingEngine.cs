using Parus.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parus.Core.Services.ElasticSearch.Indexing
{
    public abstract class ElasticIndexingEngine : IDisposable
    {
        private Thread indexingThread;

        // To get:
        // 1. bin/elastic-reset-password for user elastic
        // 2. encode elatic:<password from above step> with Base64
        // 3. Done! 
        private string password => "ZWxhc3RpYzpHUUp5YzI0U3IxdFBOdDRPc25Ccw==";
        protected string Host => "http://localhost:9200";

        private UsersIndexer usersIndexer;
        private BroadcastsIndexer broadcastIndexer;
        private ElasticTransport _transport;
        private readonly string cdnUrl;

        public ElasticIndexingEngine(string cdnUrl)
        {
            this.cdnUrl = cdnUrl;
        }

        // 5 minutes
        private TimeSpan interval = new TimeSpan(0, 5, 0);

        protected List<Indexer> IndexingQueue = new List<Indexer>();

        public void Run()
        {
            try
            {
                _transport = new ElasticTransport(Host, auth: ("Basic", password));

                int queueLength = IndexingQueue.Count;
                Task[] tasks = new Task[queueLength];

                for (int i = 0; i < queueLength; i++)
                {
                    IndexingQueue[i].Transport = _transport;
                }

                while (true)
                {
                    for (int i = 0; i < queueLength; i++)
                    {
                        Indexer indexingAction = IndexingQueue[i];

                        tasks[i] = indexingAction.RunIndexing();
                    }

                    Task.WaitAll(tasks);

                    Thread.Sleep(interval);

                    Array.Clear(tasks, 0, queueLength);
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        public void Dispose()
        {
        
        }
    }
}
