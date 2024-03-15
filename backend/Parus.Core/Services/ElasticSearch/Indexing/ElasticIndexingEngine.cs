using MimeKit.Encodings;
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
        //protected string Host => "http://localhost:9200";
        public string Host { get; protected set; }

        public bool BulkModeEnabled { get; set; }

        private UsersIndexer usersIndexer;
        private BroadcastsIndexer broadcastIndexer;
        private ElasticTransport _transport;
        private readonly string _cdnUrl;

        public ElasticIndexingEngine(bool bulkMode)
        {
            BulkModeEnabled = bulkMode;
        }

        // 5 minutes
        private TimeSpan interval = new TimeSpan(0, 5, 0);

        protected List<Indexer> IndexingQueue = new List<Indexer>();

        public void Run()
        {
            _transport = new ElasticTransport(Host, auth: ("Basic", password));

            if (CheckConnection(_transport))
            {
                //Console.WriteLine("ElasticSearch service is ready to use.");
            }
            else
            {
                throw new Exception("Error. Elastic service is not running.");
            }

            int queueLength = IndexingQueue.Count;
            Task[] tasks = new Task[queueLength];

            for (int i = 0; i < queueLength; i++)
            {
                IndexingQueue[i].Transport = _transport;
            }

            if (BulkModeEnabled)
            {
                RunInBulk(queueLength, tasks, _transport);
            }
            else
            {
                RunRegular(queueLength, tasks);
            }
        }

        private bool CheckConnection(ElasticTransport _transport)
        {
            // headering "/"
            return _transport.Header() == System.Net.HttpStatusCode.OK;
        }

        private void RunInBulk(int queueLength, Task[] tasks, ElasticTransport transport)
        {
            while (true)
            {
                for (int i = 0; i < queueLength; i++)
                {
                    Indexer indexingAction = IndexingQueue[i];

                    tasks[i] = indexingAction.RunIndexingInBulk();
                }

                Task.WaitAll(tasks);

                Thread.Sleep(interval);

                Array.Clear(tasks, 0, queueLength);
            }
        }

        private void RunRegular(int queueLength, Task[] tasks)
        {
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

        public void Dispose()
        {
        
        }
    }
}
