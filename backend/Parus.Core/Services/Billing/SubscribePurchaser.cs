using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Services.Billing
{
    public class SubscribePurchaser
    {
        private readonly string _connectionString;

        public SubscribePurchaser(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Run()
        {
            ISubscribeSessionsRepository subscribeSessions = SubscribeSessions;

            Task purchasingSubscribesTask = PurchaseSubscribes(subscribeSessions);
        }

        private Task PurchaseSubscribes(ISubscribeSessionsRepository subscribeSessions)
        {
            IEnumerable<SubscribeSession> expiringSoon = subscribeSessions.GetExpiringSoon(1, 1);

            return Task.CompletedTask;
        }

        protected virtual ISubscribeSessionsRepository SubscribeSessions { get; set; }
    }
}
