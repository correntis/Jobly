using Microsoft.AspNetCore.Http;

namespace MessagesService.Presentation.Abstractions
{
    public interface IAuthorizationHandler
    {
        Task<bool> HandleAsync(HttpContext context, IEnumerable<string> roles);
    }
}
