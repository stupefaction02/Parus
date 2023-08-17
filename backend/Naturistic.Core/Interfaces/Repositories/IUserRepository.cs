using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
		IEnumerable<IUser> Users { get; }
		bool CheckIfEmailExists(string email);
        bool CheckIfNicknameExists(string nickname);
		Task<bool> DeleteAsync(string username);
		IUser FindUserByEmail(string email);
        IUser FindUserByUsername(string nickname);
        void Update(Action value);
    }

    public interface IConfrimCodesRepository
    {
        IEnumerable<IConfirmCode> Codes { get; }

        void Add(IConfirmCode email);
    }
}