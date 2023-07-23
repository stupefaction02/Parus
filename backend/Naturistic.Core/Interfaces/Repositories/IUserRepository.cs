using System;
using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        bool CheckIfEmailExists(string email);
        bool CheckIfNicknameExists(string nickname);
        IUser FindUserByEmail(string email);
        void Update(Action value);
    }

    public interface IConfrimCodesRepository
    {
        IEnumerable<ConfirmCodeEntity> Codes { get; }

        void Add(ConfirmCodeEntity email);
    }
}