using VacanciesService.Domain.Models;

namespace VacanciesService.Domain.Abstractions.Services
{
    public interface IAuthorizationService
    {
        Task<TokenValidationResult> ValidateTokenAsync(string accessToken, string refreshToken, IEnumerable<string> roles, CancellationToken token = default);
    }
}