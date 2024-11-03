using System.Net;
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
            catch (EntityNotFoundException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                await WriteErrorToResponseAsync(context, ex);
            }
            catch (EntityAlreadyExistsException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;

                await WriteErrorToResponseAsync(context, ex);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await WriteErrorToResponseAsync(context, ex);
            }
        }

        private async Task WriteErrorToResponseAsync(HttpContext context, Exception ex)
        {
            _logger.LogError("Error: {ex}", ex.ToString());

            context.Response.ContentType = "application/json";

            var responseBody = new ApiException(context.Response.StatusCode, ex.Message, ex.ToString());

            await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody));
        }
    }
}
