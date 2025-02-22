using MessagesService.Core.Models;

namespace MessagesService.DataAccess.Abstractions
{
    public interface IAuthorizationService
    {
        Task<TokenValidationResult> ValidateTokenAsync(string accessToken, string refreshToken, IEnumerable<string> roles, CancellationToken token = default);
    }
}