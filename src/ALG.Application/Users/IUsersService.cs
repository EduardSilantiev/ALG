using ALG.Application.Users.Dto;
using System.Threading.Tasks;

namespace ALG.Application.Users
{
    public interface IUsersService
    {
        Task<UserDto> LoginAsync(CredentialsDto credentialsDto);
    }
}
