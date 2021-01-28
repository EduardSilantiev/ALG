using ALG.Core.Users;
using AutoMapper;

namespace ALG.Application.Users.Dto
{
    public class UsersMapProfile : Profile
    {
        public UsersMapProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
