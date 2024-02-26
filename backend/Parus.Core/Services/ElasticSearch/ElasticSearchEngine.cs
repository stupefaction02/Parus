using Parus.Core.Interfaces.Repositories;
using System;
using System.Threading;

namespace Parus.Core.Services.ElasticSearch
{
    public class ElasticSearchEngine : IDisposable
    {
        private Thread indexingThread;

        // To get:
        // 1. bin/elastic-reset-password for user elastic
        // 2. encode elatic:<password from above step> with Base64
        // 3. Done!
        private string password => "ZWxhc3RpYzpHUUp5YzI0U3IxdFBOdDRPc25Ccw==";
        private string host => "http://localhost:9200";

        private UsersIndexer usersIndexer;
        private BroadcastsIndexer broadcastIndexer;
        private ElasticTransport transport;

        public ElasticSearchEngine(string cdnUrl, 
            Func<IUserRepository> getUsers, 
            Func<IBroadcastInfoRepository> getBroadcasts)//IUserRepository users)
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

            this.transport = transport;

            this.indexingThread.Start();

            usersIndexer = new UsersIndexer(getUsers, cdnUrl, transport);
            broadcastIndexer = new BroadcastsIndexer(getBroadcasts, transport);
        }

        public void RunIndexing()
        {
            var indexingUsersTask = usersIndexer.RunIndexing();
            var indexingUsersTask = usersIndexer.RunIndexing();

            while (true)
            {

                // update configs
                Thread.Sleep(1000);
            }
        }

        private struct ElasticUserModel
        {
            public string Username;
            public string Avapath;
            public string Description;
        }

        public struct Setting
        {
            public string Name;
            public object Value;
        }

        public void Dispose()
        {
            indexingThread.Abort();

            indexingThread = null;
        }
    }
}
