using Microsoft.AspNetCore.Http;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Constants;
using UsersService.Domain.Enums;
using UsersService.Presentation.Abstractions;

namespace UsersService.Presentation.Middleware.Authentication
{
    public class AuthorizationHandler : IAuthorizationHandler
    {
        private readonly IAuthorizationService _authService;

        public AuthorizationHandler(IAuthorizationService authService)
        {
            _authService = authService;
        }

        public async Task<bool> HandleAsync(HttpContext context, IEnumerable<string> roles)
        {
            if (!context.Request.Cookies.TryGetValue(BusinessRules.Token.AccessTokenName, out string accessToken))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return false;
            }

            if (!IsValidToken(accessToken, out TokenValidationResults tokenValidationResult))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return false;
            }

            if (!IsValidRoles(accessToken, roles))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                return false;
            }

            if (tokenValidationResult == TokenValidationResults.SuccessExpired)
            {
                return await TryRefreshExpiredToken(context);
            }

            return true;
        }

        private bool IsValidToken(string token, out TokenValidationResults result)
        {
            result = _authService.ValidateToken(token);

            return result != TokenValidationResults.Failure;
        }

        private bool IsValidRoles(string token, IEnumerable<string> roles)
        {
            return _authService.ValidateRoles(token, roles) != TokenValidationResults.Failure;
        }

        private async Task<bool> TryRefreshExpiredToken(HttpContext context)
        {
            if (!context.Request.Cookies.TryGetValue(BusinessRules.Token.RefreshTokenName, out string refreshToken))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return false;
            }

            var accessToken = await _authService.RefreshTokenAsync(refreshToken, context.RequestAborted);

            if (accessToken == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return false;
            }

            UpdateAccessTokenInCookie(context, accessToken);

            return true;
        }

        private void UpdateAccessTokenInCookie(HttpContext context, string accessToken)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddDays(BusinessRules.Token.AccessTokenExpiresDays),
            };

            context.Response.Cookies.Append(BusinessRules.Token.AccessTokenName, accessToken, cookieOptions);
        }
    }
}
