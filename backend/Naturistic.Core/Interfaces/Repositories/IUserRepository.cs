using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        //IEnumerable<Naturistic.Infrastructure.Identity.ApplicationUser> Users { get; }

        bool CheckIfEmailExists(string email);
        bool CheckIfNicknameExists(string nickname);
    }

    public interface IConfrimCodesRepository
    {
        IEnumerable<ConfirmCodeEntity> Codes { get; }

        void Add(ConfirmCodeEntity email);
    }
}