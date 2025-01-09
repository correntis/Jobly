using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Presentation.Middleware
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
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await HandleExceptionAsync(context, ex);
            }
            catch (EntityNotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await HandleExceptionAsync(context, ex);
            }
            catch (EntityAlreadyExistException ex)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, string details = null)
        {
            LogException(ex, details);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(GetApiException(context.Response.StatusCode, ex, details));
        }

        private void LogException(Exception ex, string details)
        {
            _logger.LogError("An error of type {ExceptionType} occured: {Exception}", ex.GetType(), details ?? ex.ToString());
        }

        private ApiException GetApiException(int statusCode, Exception ex, string details)
        {
            return new ApiException(statusCode, ex.Message, details ?? ex.ToString());
        }
    }
}
