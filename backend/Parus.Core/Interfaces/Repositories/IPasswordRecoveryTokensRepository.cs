using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parus.Core.Entities;

namespace Parus.Core.Interfaces.Repositories
{
    public interface IPasswordRecoveryTokensRepository
    {
        IEnumerable<IPasswordRecoveryToken> Tokens { get; }

        void Add(IPasswordRecoveryToken token);
        void Delete(IPasswordRecoveryToken token);
        void DeleteAll(Func<IUser, bool> token);
        IUser GetUser(string token);
        bool Contains(string userId);
		IPasswordRecoveryToken OneByUser(string userId);
		void ClearTracking();
		Task DeleteAsync(IPasswordRecoveryToken token);
        IPasswordRecoveryToken GetTokenWithUser(string token);
    }

    public interface IPasswordRecoveryToken
    {
        long GetExpiresAt();
        string GetToken();
        IUser GetUser();
        bool Validate(out string errorMessage);
    }
}