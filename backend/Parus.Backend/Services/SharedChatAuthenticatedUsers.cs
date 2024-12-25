using Parus.Infrastructure.Identity;
using System.Collections.Concurrent;

namespace Parus.API.Services
{
    public class SharedChatAuthenticatedUsers
    {
        private ConcurrentDictionary<string, ParusUser> _chatAuthenticatedUsers = new();

        public ParusUser this[string key] 
        {
            get
            {
                return _chatAuthenticatedUsers[key];
            }
            set 
            {
                _chatAuthenticatedUsers[key] = value;
            }
        }

        public bool TryGet(string key, out ParusUser user)
        {
            return _chatAuthenticatedUsers.TryGetValue(key, out user);
        }

        public bool Set(string key, ParusUser user)
        {
            return _chatAuthenticatedUsers.TryAdd(key, user);
        }

        public bool Unset(string key, out ParusUser user)
        {
            return _chatAuthenticatedUsers.TryRemove(key, out user);
        }
    }
}
