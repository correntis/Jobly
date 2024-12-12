using Microsoft.AspNetCore.Http;

namespace VacanciesService.Presentation.Abstractions
{
    public interface IAuthorizationHandler
    {
        Task<bool> HandleAsync(HttpContext context, IEnumerable<string> roles);
    }
}