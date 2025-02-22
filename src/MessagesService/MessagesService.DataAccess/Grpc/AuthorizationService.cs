using Jobly.Protobufs.Authorization;
using Jobly.Protobufs.Authorization.Client;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.DataAccess.Grpc
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AuthorizationGrpcService.AuthorizationGrpcServiceClient _client;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(
            ILogger<AuthorizationService> logger,
            AuthorizationGrpcService.AuthorizationGrpcServiceClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<TokenValidationResult> ValidateTokenAsync(
            string accessToken,
            string refreshToken,
            IEnumerable<string> roles,
            CancellationToken token = default)
        {
            var request = new ValidateTokenRequest()
            {
                AccessToken = accessToken ?? string.Empty,
                RefreshToken = refreshToken ?? string.Empty,
            };

            request.RequiredRoles.AddRange(roles);

            _logger.LogInformation(
                "[GRPC] Start handling {RequestName} {@RequestBody}",
                typeof(ValidateTokenRequest).Name,
                request);

            var response = await _client.ValidateTokenAsync(request, cancellationToken: token);

            _logger.LogInformation(
                "[GRPC] Successfully handled {RequestName} with request {@ResponseBody}",
                typeof(ValidateTokenRequest).Name,
                response);

            return new TokenValidationResult(
                response.IsValidToken,
                response.IsValidRoles,
                response.IsAccessTokenRefreshed,
                response.NewAccessToken);
        }
    }
}
