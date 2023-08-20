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
		bool Contains(Func<IUser, bool> predicate);
		void RemoveOne(string username);
		IUser FindUserByEmail(string email);
        IUser FindUserByUsername(string nickname);
		IUser One(Func<IUser, bool> predicate);
		void Update(Action value);
		bool Update(IUser user);
		void ClearTracking();
	}

    public interface IConfrimCodesRepository
    {
        IEnumerable<IConfirmCode> Codes { get; }

        void Add(IConfirmCode email);
		void ClearTracking();
		bool Contains(string userId);
		IConfirmCode OneByUser(string id);
		int Remove(IConfirmCode token);
	}
}