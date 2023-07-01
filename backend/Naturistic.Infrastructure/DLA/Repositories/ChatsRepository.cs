using System.Linq;
using System.Collections.Generic;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class ChatsRepository : IChatsRepository
    {
        private readonly ApplicationDbContext context;
        public IEnumerable<Chat> Chats => context.Chats;

        public ChatsRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Chat Get(int id)
        {
            return context.Chats.Single(c => c.ChatId == id);
        }

        public Chat GetOwner(int id)
        {
            var chat = context.Chats.SingleOrDefault(c => c.ChatId == id);

            context.Entry(chat).Reference(x => x.BroadcastUser);

            return chat;
        }

        public IEnumerable<Message> GetMessages(int id)
        {
            return context.Chats.Include(x => x.Messages)
                    .SingleOrDefault(c => c.ChatId == id)
                    .Messages;
        }

        public void Add(Chat chat)
        {
            context.Chats.Add(chat);
            context.SaveChanges();
        }
    }
}