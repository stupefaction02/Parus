using Parus.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Parus.Core.Entities
{
    /* Billing */
    // int finalWithdrawal = (currency * sUnit)
    // universal unit

    public class SubscriptionProfile
    {
        [Key]
        public int SubscriptionProfileId { get; set; }

        public string Name { get; set; }

        public int DurationDays { get; set; }

        public int PriceUnit { get; set; }

        public string Description { get; set; }

        public List<SubscriptionSession> Sessions { get; set; }
    }

    public class UnitPrice
    {
        [Key]
        public int UnitPriceId { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }
    }

    public class BillingCachingResults
    {
        public Dictionary<string, SubscriptionProfile> Profiles = new Dictionary<string, SubscriptionProfile>();
    }

    public enum SubscriptionSessionStatus
    {
        Active = 1,
        Expired = 2,
        Paused = 3,
        NonActive = 4
    }

    public class SubscriptionSession
    {
        public int SubscriptionSessionId { get; set; }

        public bool Autocontinuation { get; set; }

        public int ProfileId { get; set; }

        public SubscriptionProfile Profile { get; set; }

        public SubscriptionSessionStatus Status { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string PurchaserUserId { get; set; }

        public int BroadcasterId { get; set; }

        public override string ToString()
        {
            return $"{PurchaserUserId} {BroadcasterId} {ExpiresAt}";
        }
    }
}
