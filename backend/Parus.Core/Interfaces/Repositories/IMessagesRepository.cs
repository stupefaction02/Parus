using System.Collections.Generic;
using Parus.Core.Entities;

namespace Parus.Core.Interfaces.Repositories
{
    public interface IMessagesRepository
    {
        IEnumerable<ChatMessage> Messages { get; }

        void Add(ChatMessage message);
    }
}