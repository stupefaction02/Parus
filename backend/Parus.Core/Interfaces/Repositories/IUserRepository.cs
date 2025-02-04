﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parus.Core.Entities;

namespace Parus.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
		IEnumerable<IUser> Users { get; }
		bool IsEmailTaken(string email);
        bool IsUsernameTaken(string nickname);
		bool Contains(Func<IUser, bool> predicate);
		void RemoveOne(string username);
		IUser FindUserByEmail(string email);
        IUser FindUserByUsername(string nickname);
		IUser One(Func<IUser, bool> predicate);
		void Update(Action value);
		bool Update(IUser user);
		void ClearTracking();
        void UpdateWithoutContextSave(IUser user);
        int SaveChanges();
        int GetUserRegionId(string userId);
    }

    public interface IConfrimCodesRepository
    {
        IEnumerable<IVerificationCode> Codes { get; }

        void Add(IVerificationCode email);
		void ClearTracking();
		bool Contains(string userId);
		IVerificationCode OneByUser(string id);
		int Remove(IVerificationCode token);
	}
}