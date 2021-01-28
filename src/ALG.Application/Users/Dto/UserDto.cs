using System;

namespace ALG.Application.Users.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string TokenExpires { get; set; }
        public string Role { get; set; }

    }
}
