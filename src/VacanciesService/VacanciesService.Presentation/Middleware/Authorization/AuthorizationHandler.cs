using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Presentation.Abstractions;

namespace VacanciesService.Presentation.Middleware.Authorization
{
    public class AuthorizationHandler : IAuthorizationHandler
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AuthorizationHandler> _logger;
        private readonly IAuthorizationService _authService;

        public AuthorizationHandler(
            IWebHostEnvironment environment,
            ILogger<AuthorizationHandler> logger,
            IAuthorizationService authService)
        {
            _environment = environment;
            _logger = logger;
            _authService = authService;
        }

        public async Task<bool> HandleAsync(HttpContext context, IEnumerable<string> roles)
        {
            _logger.LogDebug("[DEBUG] Start handle auhorization from {Host} {@Cookies}", context.Request.Host, context.Request.Cookies);

            // 401 code return has been temporarily removed to simplify development.
            // In other cases, if there is no token in the cookie, you don’t even have to request the authorization service

            context.Request.Cookies.TryGetValue(BusinessRules.Token.AccessTokenName, out string accessToken); // Only development

            if (!_environment.IsDevelopment())
            {
                if (!context.Request.Cookies.TryGetValue(BusinessRules.Token.AccessTokenName, out accessToken))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return false;
                }
            }

            context.Request.Cookies.TryGetValue(BusinessRules.Token.AccessTokenName, out string refreshToken);

            var tokenValidationResult =
                await _authService.ValidateTokenAsync(accessToken, refreshToken, roles, context.RequestAborted);

            if (!tokenValidationResult.IsValidToken)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return false;
            }

            if (!tokenValidationResult.IsValidRoles)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return false;
            }

            if (tokenValidationResult.IsAccessTokenRefreshed)
            {
                UpdateAccessTokenInCookie(context, tokenValidationResult.NewAccessToken);
            }

            return true;
        }

        private void UpdateAccessTokenInCookie(HttpContext context, string accessToken)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(BusinessRules.Token.AccessTokenExpiresDays),
            };

            context.Response.Cookies.Append(BusinessRules.Token.AccessTokenName, accessToken, cookieOptions);
        }
    }

}
