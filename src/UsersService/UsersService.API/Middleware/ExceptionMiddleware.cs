using System.Text.Json;
using UsersService.Domain.Exceptions;

namespace UsersService.API.Middleware
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

                await WriteExceptionToResponseAsync(context, ex, JsonSerializer.Serialize(ex.Errors));
            }
            catch (EntityNotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await WriteExceptionToResponseAsync(context, ex);
            }
            catch (EntityAlreadyExistsException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;

                await WriteExceptionToResponseAsync(context, ex);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await WriteExceptionToResponseAsync(context, ex);
            }
        }

        private async Task WriteExceptionToResponseAsync(HttpContext context, Exception ex)
        {
            _logger.LogError("Error: {ex}", ex.ToString());

            context.Response.ContentType = "application/json";

            var apiException = new ApiException(context.Response.StatusCode, ex.Message, ex.ToString());

            await context.Response.WriteAsJsonAsync(apiException);
        }

        private async Task WriteExceptionToResponseAsync(HttpContext context, Exception ex, string details)
        {
            _logger.LogError("Error: {ex}", details);

            context.Response.ContentType = "application/json";

            var apiException = new ApiException(context.Response.StatusCode, ex.Message, details);

            await context.Response.WriteAsJsonAsync(apiException);
        }
    }
}
