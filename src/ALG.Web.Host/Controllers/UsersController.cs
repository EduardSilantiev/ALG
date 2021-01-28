using ALG.Application.Helpers.Exceptions;
using ALG.Application.Users;
using ALG.Application.Users.Dto;
using ALG.Web.Host.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace ALG.Web.Host.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Action to Login into the system
        /// </summary>
        /// <param name="credentialsDto">Model with user credentials</param>
        /// <returns>Returns the authenticated User</returns>
        /// <response code="200">Returned if the user was authenticated</response>
        /// <response code="400">Returned if the user authentication failed (invalid Email or Password)</response>
        /// <response code="422">Returned when the model validation failed</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerRequestExample(typeof(CredentialsDto), typeof(UserLoginRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserLoginOkResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UserLoginBadRequestResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status422UnprocessableEntity, typeof(UserLoginUnprocessableEntityResponseExample))]
        public async Task<IActionResult> LoginAsync(CredentialsDto credentialsDto)
        {
            var validationResult = new CredentialsDtoValidator().Validate(credentialsDto);
            if (!validationResult.IsValid)
                return UnprocessableEntity(new ExceptionMessage(validationResult.Errors.Select(x => x.ErrorMessage)));

            var authenticatedDto = await _usersService.LoginAsync(credentialsDto);
            if (authenticatedDto == null)
                return BadRequest(new ExceptionMessage("The Email or Password is incorrect."));

            return Ok(authenticatedDto);
        }
    }
}
