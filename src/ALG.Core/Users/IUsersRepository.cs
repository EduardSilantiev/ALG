using System.Threading.Tasks;

namespace ALG.Core.Users
{
    public interface IUsersRepository
    {
        Task<User> GetUserByEmailAsync(string email);
    }
}
