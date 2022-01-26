using System;
using System.Collections.Generic;
using Naturistic.Core.Entities.Chat;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Backend.Entities
{
    public class Chat
    {
        public int Id { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public List<Message> Messages { get; set; }
    }
}