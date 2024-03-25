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

        long clockslew = 60 * 60;

        public void Run()
        {
            ISubscribeSessionsRepository subscribeSessions = SubscribeSessions;

            /* Norificate users */
            Task purchasingSubscribesTask = PurchaseSubscribes(subscribeSessions);

            /* Subscribe users */
            Task notificateTask = NotificateAboutExpiring(subscribeSessions);

            Task.WaitAll(purchasingSubscribesTask, notificateTask);
        }

        private Task NotificateAboutExpiring(ISubscribeSessionsRepository subscribeSessions)
        {
            // 12.52 23.03.2024
            long currenTimestamp = 1711187568;
            
            IEnumerable<SubscribeSession> expiringIn1Day = subscribeSessions.GetExpiringSoon(currenTimestamp, 60 * 60 * 24);

            return Task.CompletedTask;
        }

        private Task PurchaseSubscribes(ISubscribeSessionsRepository subscribeSessions)
        {
            // 12.52 23.03.2024
            long currenTimestamp = 1711187568;

            IEnumerable<SubscribeSession> expiringSoon = subscribeSessions.GetExpiringSoon(currenTimestamp, clockslew);

            return Task.CompletedTask;
        }

        protected virtual ISubscribeSessionsRepository SubscribeSessions { get; set; }
    }
}
