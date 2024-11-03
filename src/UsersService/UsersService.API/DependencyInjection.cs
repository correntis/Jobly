using UsersService.API.Middleware;

namespace UsersService.API
{
    public static class DependencyInjection
    {
        public static void AddPresentation(this IServiceCollection services)
        {
            services.AddScoped<ExceptionMiddleware>();
        }

        public static void UsePresentation(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
