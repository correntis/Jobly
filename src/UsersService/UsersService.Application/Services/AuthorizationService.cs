using Microsoft.Extensions.Logging;
using System.Text.Json;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Enums;
using UsersService.Domain.Models;

namespace UsersService.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ILogger<AuthorizationService> _logger;
        private readonly ITokensService _tokensService;
        private readonly ITokensRepository _tokensRepository;

        public AuthorizationService(
            ILogger<AuthorizationService> logger,
            ITokensService tokensService,
            ITokensRepository tokensRepository)
        {
            _logger = logger;
            _tokensService = tokensService;
            _tokensRepository = tokensRepository;
        }

        public async Task<Token> IssueTokenAsync(int id, IEnumerable<string> roles, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start issue token for user with ID {UserId} and roles {@UserRoles}", id, roles);

            var refreshToken = _tokensService.CreateRefreshToken();
            var accessToken = _tokensService.CreateAccessToken(
                id,
                roles,
                DateTime.UtcNow.AddDays(BusinessRules.Token.AccessTokenExpiresDays));

            var tokenEntity = new TokenEntity()
            {
                RefreshToken = refreshToken,
                UserId = id,
                UserRoles = roles,
            };

            await _tokensRepository.SetAsync(
                refreshToken,
                JsonSerializer.Serialize(tokenEntity),
                DateTime.UtcNow.AddDays(BusinessRules.Token.RefreshTokenExpiresDays),
                cancellationToken);

            var token = new Token()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            _logger.LogInformation("Token successfully issued for user with ID {UserId} and roles {@UserRoles}", id, roles);

            return token;
        }

        public async Task<string> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start refresh access token with refresh token");

            var tokenEntityString = await _tokensRepository.GetAsync(refreshToken, cancellationToken);

            if (tokenEntityString is null)
            {
                return null;
            }

            var tokenEntity = JsonSerializer.Deserialize<TokenEntity>(tokenEntityString);

            var accessToken = _tokensService.CreateAccessToken(
                tokenEntity.UserId,
                tokenEntity.UserRoles,
                DateTime.UtcNow.AddDays(BusinessRules.Token.AccessTokenExpiresDays));

            _logger.LogInformation(
                "Successfully refreshed access token for user with ID {UserId} and roles {@UserRoles}",
                tokenEntity.UserId,
                tokenEntity.UserRoles);

            return accessToken;
        }

        public TokenValidationResults ValidateToken(string accessToken)
        {
            return _tokensService.ValidateToken(accessToken);
        }

        public TokenValidationResults ValidateRoles(string accessToken, IEnumerable<string> roles)
        {
            return _tokensService.ValidateRoles(accessToken, roles);
        }
    }
}
