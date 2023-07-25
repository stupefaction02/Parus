using System;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Entities
{

    // Hosts broadcast, can edit channel
    public class BroadcastUser
    {
        public virtual Chat Chat { get; set; }

        public int BroadcastUserId { get; set; }

        public string IdentityUserId { get; set; }
    }

    public interface IUser
    {
        public string GetUsername();

        public string GetEmail();

        public bool EmailConfirmed { get; set; }
    }
}