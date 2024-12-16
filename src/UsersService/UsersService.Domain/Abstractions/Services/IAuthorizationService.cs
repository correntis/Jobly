using UsersService.Domain.Enums;
using UsersService.Domain.Models;

namespace UsersService.Domain.Abstractions.Services
{
    public interface IAuthorizationService
    {
        Task<Token> IssueTokenAsync(Guid id, IEnumerable<string> roles, CancellationToken cancellationToken = default);
        Task<string> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        TokenValidationResults ValidateToken(string accessToken);
        TokenValidationResults ValidateRoles(string accessToken, IEnumerable<string> roles);
    }
}