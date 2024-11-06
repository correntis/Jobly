namespace UsersService.API.Abstractions
{
    public interface IAuthorizationHandler
    {
        Task<bool> HandleAsync(HttpContext context, IEnumerable<string> roles);
    }
}