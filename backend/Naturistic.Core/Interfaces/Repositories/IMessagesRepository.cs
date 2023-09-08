using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IMessagesRepository
    {
        IEnumerable<ChatMessage> Messages { get; }

        void Add(ChatMessage message);
    }
}