using Parus.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Parus.Core.Entities
{
    public class SubscribeProfile
    {
        [Key]
        public int Key { get; set; }

        public string Name { get; set; }

        // months
        public int Duration { get; set; }

        // universal unit
        public int Price { get; set; }
    }

    public class BillingCachingResults
    {
        public Dictionary<string, SubscribeProfile> Profiles = new Dictionary<string, SubscribeProfile>();
    }

    public enum SubscribeSessionStatus
    {
        Active = 1,
        Expired = 2,
    }

    public class SubscribeSession
    {
        [Key]
        public int Key { get; set; }

        public bool Autocontinuation { get; set; }

        public SubscribeProfile Profile { get; set; } = null!;

        public SubscribeSessionStatus Status { get; set; }

        public long ExpiresAt { get; set; } = 0!;

        public string PurchaserUserId { get; set; } = null!;
        public string SubjectUserId { get; set; } = null!;

        public virtual IUser GetSubject() { return default; }
        public virtual IUser GetPurchaser() { return default; }

        public override string ToString()
        {
            return "March 24";
        }
    }
}
