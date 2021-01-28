using ALG.Application.Helpers.Exceptions;
using ALG.Application.Helpers.Paging.Dto;
using ALG.Application.Services;
using ALG.Application.Services.Dto;
using ALG.Web.Host.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ALG.Web.Host.Controllers
{
    /// <response code="401">Returned when the user is not authenticated</response>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(Status401UnauthorizednResponseExample))]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        /// <summary>
        /// Action to obtaine a list of services with its paging information
        /// </summary>
        /// <param name="filter">A string to filter services by name. No filtering if 'filter' is not provided</param>
        /// <param name="currentPage">Current page for paging implementation. currentPage = 1 by default</param>
        /// <param name="pageSize">Page size for paging implementation. pageSize = default value from appsettings;
        /// if pageSize = -1,  pageSize = the size of a whole collection 
        /// </param>
        /// <returns>Returns the paginated list of services</returns>
        /// <response code="200">Returned if the list is obtained</response>
        /// <response code="422">Returned when the model validation failed</response>
        [HttpGet()]
        [ProducesResponseType(typeof(ServiceListDto), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ServiceListDtoResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status422UnprocessableEntity, typeof(GetListOfServicesUnprocessableEntityResponseExample))]
        public async Task<IActionResult> GetServiceListAsync(string filter, int currentPage, int pageSize)
        {
            var getServicesListDto = new GetServicesListDto
            {
                Filter = filter,
                CurrentPage = currentPage,
                PageSize = pageSize
            };
            var validationResult = new GetPaginatedListDtoValidator().Validate(getServicesListDto);
            if (!validationResult.IsValid)
                return UnprocessableEntity(new ExceptionMessage(validationResult.Errors.Select(x => x.ErrorMessage)));

            var userId = Guid.Parse(User.Identity.Name);
            var servicesListDtos = await _servicesService.GetAllServicesAsync(userId, getServicesListDto);
            
            return Ok(servicesListDtos);
        }

        /// <summary>
        /// Action to activate a bonus for a service
        /// </summary>
        /// <param name="activateBonusDto">Model with a bonus activation data</param>
        /// <returns>No content</returns>
        /// <response code="204">Returned if the the bonus is activated</response>
        /// <response code="400">Returned if a service not found or a promocode is invalid for the service</response>
        /// <response code="409">Returned if the the bonus is already activated</response>
        /// <response code="422">Returned when the model validation failed</response>
        [HttpPut("activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerRequestExample(typeof(ActivateBonusDto), typeof(ActivateBonusRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ActivateBonusBadRequestResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(ActivateBonusConflictRequestResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status422UnprocessableEntity, typeof(ActivateUnprocessableEntityResponseExample))]
        public async Task<IActionResult> ActivateBonusAsync(ActivateBonusDto activateBonusDto)
        {
            var validationResult = new ActivateBonusDtoValidator().Validate(activateBonusDto);
            if (!validationResult.IsValid)
                return UnprocessableEntity(new ExceptionMessage(validationResult.Errors.Select(x => x.ErrorMessage)));

            var userId = Guid.Parse(User.Identity.Name);
            var canBeProcessedResult = await _servicesService.BonusCanBeActivatedAsync(userId, activateBonusDto);
            if (!canBeProcessedResult.CanBeProcessed)
                return StatusCode(canBeProcessedResult.StatusCode, new ExceptionMessage(canBeProcessedResult.RejectionReason));

            await _servicesService.ActivateBonusAsync(userId, activateBonusDto);
            return NoContent();
        }
    }
}
