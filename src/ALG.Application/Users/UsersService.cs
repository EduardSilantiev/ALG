using ALG.Application.Settings;
using ALG.Application.Users.Dto;
using ALG.Core.Users;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ALG.Application.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly JWTSettings _settings;

        public UsersService(IUsersRepository usersRepository, IMapper mapper, IOptions<JWTSettings> settings)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _settings = settings.Value;
        }

        /// <summary>
        /// Performs login into the system
        /// </summary>
        /// <param name="credentials">Email and Password for login</param>
        /// <returns>User data if logged in or null if login failed</returns>
        public async Task<UserDto> LoginAsync(CredentialsDto credentials)
        {
            var user = await _usersRepository.GetUserByEmailAsync(credentials.Email);
            if (user == null) return null;

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, credentials.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            var userDto = _mapper.Map<UserDto>(user);

            var tokenResult = GenerateToken(user.Id.ToString(), user.Role);
            userDto.AccessToken = tokenResult.token;
            userDto.TokenExpires = tokenResult.expires.ToString("yyyy-MM-ddTHH:mm:ss");

            return userDto;
        }

        /// <summary>
        /// Generated JWT token
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="role">User role</param>
        /// <returns>A token and its expiration Date and Time</returns>
        private (string token, DateTime expires) GenerateToken(string userId, string role)
        {
            var expires = DateTime.Now.AddHours(_settings.TokenValidHrs);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_settings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return (tokenString, expires);
        }
    }
}
