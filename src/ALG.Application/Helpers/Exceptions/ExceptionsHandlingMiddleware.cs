using ALG.Application.Settings;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ALG.Application.Helpers.Exceptions
{
    public class ExceptionsHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionsHandlingMiddleware> _logger;
        private readonly ICorsService _corsService;
        private readonly CorsOptions _corsOptions;
        private readonly RuntimeSettings _settings;


        /// <summary>
        /// Class constructor
        /// </summary>
        public ExceptionsHandlingMiddleware(RequestDelegate next, ILogger<ExceptionsHandlingMiddleware> logger,
                                            ICorsService corsService, IOptions<CorsOptions> corsOptions,
                                            IOptions<RuntimeSettings> settings)
        {
            _next = next;
            _logger = logger;
            _corsService = corsService;
            _corsOptions = corsOptions.Value;
            _settings = settings.Value;
        }

        /// <summary>
        /// Execute Middleware
        /// </summary>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleUnhandledExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Handle unhandled exceptions
        /// </summary>
        /// <param name="context">Current HttpContext</param>
        /// <param name="exception">System.Exception.</param>
        private async Task HandleUnhandledExceptionAsync(HttpContext context, Exception exception)
        {

            _logger.LogError(exception, exception.Message);

            if (!context.Response.HasStarted)
            {
                context.Response.Clear();

                //repopulate Response header with CORS policy to send the response with CORS headers
                _corsService.ApplyResult(_corsService.EvaluatePolicy(context, _corsOptions.GetPolicy("Default")),
                                                    context.Response);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var message = string.Empty;
                if (_settings.DetailedErrors)
                    message = exception.Message;
                else
                    message = "An unhandled exception has occurred.";

                //implement unified error messaging approach
                var result = new ExceptionMessage(message).ToString();
                await context.Response.WriteAsync(result);
            }
        }
    }
}
