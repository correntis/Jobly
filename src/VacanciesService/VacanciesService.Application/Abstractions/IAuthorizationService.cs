using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Abstractions
{
    public interface IAuthorizationService
    {
        Task<TokenValidationResult> ValidateTokenAsync(string accessToken, string refreshToken, IEnumerable<string> roles);
    }
}