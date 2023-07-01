using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IChatsRepository
    {
        IEnumerable<Chat> Chats { get; }

        void Add(Chat chat);
        
        Chat Get(int id);

        IEnumerable<Message> GetMessages(int id);
    }
}