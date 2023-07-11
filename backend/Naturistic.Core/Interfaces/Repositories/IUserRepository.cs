namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        bool CheckIfEmailExists(string email);
        bool CheckIfNicknameExists(string nickname);
    }
}