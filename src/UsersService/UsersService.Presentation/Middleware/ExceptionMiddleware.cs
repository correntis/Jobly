using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UsersService.Domain.Exceptions;

namespace UsersService.Presentation.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await HandleExceptionAsync(context, ex, JsonSerializer.Serialize(ex.Errors));
            }
            catch (EntityNotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await HandleExceptionAsync(context, ex);
            }
            catch (EntityAlreadyExistsException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;

                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError("An error of type {ExceptionType} occured: {Exception}", ex.GetType(), ex.ToString());

            context.Response.ContentType = "application/json";

            var apiException = new ApiException(context.Response.StatusCode, ex.Message, ex.ToString());

            await context.Response.WriteAsJsonAsync(apiException);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, string details)
        {
            _logger.LogError("An error of type {ExceptionType} occured: {Exception}", ex.GetType(), ex.ToString());

            context.Response.ContentType = "application/json";

            var apiException = new ApiException(context.Response.StatusCode, ex.Message, details);

            await context.Response.WriteAsJsonAsync(apiException);
        }
    }
}
