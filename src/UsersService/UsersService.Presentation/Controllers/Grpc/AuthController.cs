using Grpc.Core;
using Jobly.Protobufs.Authorization;
using Jobly.Protobufs.Authorization.Server;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Enums;

namespace UsersService.Presentation.Controllers.Grpc
{
    public class AuthController : AuthorizationGrpcService.AuthorizationGrpcServiceBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthorizationService _authService;

        public AuthController(
            ILogger<AuthController> logger,
            IAuthorizationService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Start proccessing gRPC request {RequestName}", request.GetType().Name);

            if (!IsValidToken(request.AccessToken, out TokenValidationResults tokenValidationResult))
            {
                _logger.LogInformation("{RequestName} failed. Invalid access token", request.GetType().Name);

                return new ValidateTokenResponse() { IsValidToken = false };
            }

            if (!IsValidRoles(request.AccessToken, request.RequiredRoles))
            {
                _logger.LogInformation(
                    "{RequestName} failed. Forbidden, required roles {RequiredRoles}",
                    request.GetType().Name,
                    request.RequiredRoles);

                return new ValidateTokenResponse() { IsValidRoles = false };
            }

            if (tokenValidationResult == TokenValidationResults.SuccessExpired)
            {
                return await HandleExpiredTokenAsync(request.RefreshToken, context.CancellationToken);
            }

            _logger.LogInformation("Successfully proccessed gRPC request {RequestName}", request.GetType().Name);

            return new ValidateTokenResponse()
            {
                IsValidRoles = true,
                IsValidToken = true,
            };
        }

        private async Task<ValidateTokenResponse> HandleExpiredTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Access token is expired. Trying to refresh");

            var newAccessToken = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);

            if (newAccessToken is null)
            {
                _logger.LogInformation("Attempt to refresh token failed");

                return new ValidateTokenResponse() { IsValidToken = false };
            }

            _logger.LogInformation("Access token successfully refreshed");

            return new ValidateTokenResponse()
            {
                IsValidToken = true,
                IsValidRoles = true,
                IsAccessTokenRefreshed = true,
                NewAccessToken = newAccessToken,
            };
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
    }
}
