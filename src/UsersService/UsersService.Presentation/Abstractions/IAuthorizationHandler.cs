using Microsoft.AspNetCore.Http;

namespace UsersService.Presentation.Abstractions
{
    public interface IAuthorizationHandler
    {
        Task<bool> HandleAsync(HttpContext context, IEnumerable<string> roles);
    }
}