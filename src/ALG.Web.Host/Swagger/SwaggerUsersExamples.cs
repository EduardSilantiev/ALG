using ALG.Application.Helpers.Exceptions;
using ALG.Application.Users.Dto;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace ALG.Web.Host.Swagger
{
    public class UserLoginRequestExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new CredentialsDto()
            {
                Email = "john.dow@gmail.com",
                Password = "111"
            };
    }

    public class UserLoginOkResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new UserDto()
            {
                Id = Guid.NewGuid(),
                Name = "John, Dow",
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bml...",
                TokenExpires = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Role = "User"
            };
    }

    public class UserLoginBadRequestResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ExceptionMessage("The Email or Password is incorrect.");
    }

    public class Status401UnauthorizednResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ExceptionMessage("User is not authenticated.");
    }

    public class UserLoginUnprocessableEntityResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() =>
            new ExceptionMessage("'Email' must not be empty.; 'Password' must not be empty.");
    }
}
