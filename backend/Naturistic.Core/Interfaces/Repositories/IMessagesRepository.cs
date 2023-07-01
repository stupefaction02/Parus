using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IMessagesRepository
    {
        IEnumerable<Message> Messages { get; }

        void Add(Message message);
    }
}