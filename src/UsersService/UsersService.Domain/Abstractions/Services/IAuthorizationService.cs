using UsersService.Domain.Enums;
using UsersService.Domain.Models;

namespace UsersService.Domain.Abstractions.Services
{
    public interface IAuthorizationService
    {
        Task<Token> IssueTokenAsync(int id, IEnumerable<string> roles, CancellationToken cancellationToken);
        Task<string> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        TokenValidationResults ValidateToken(string accessToken);
        TokenValidationResults ValidateRoles(string accessToken, IEnumerable<string> roles);
    }
}