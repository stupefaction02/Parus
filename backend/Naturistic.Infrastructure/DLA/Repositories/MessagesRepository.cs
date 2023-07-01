using System;
using System.Collections.Generic;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly ApplicationDbContext context;

        public IEnumerable<Message> Messages => context.Messages; 

        public MessagesRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(Message channel)
        {
            context.Messages.Add(channel);
            context.SaveChanges();
        }
    }
}