using UsersService.API.Abstractions;
using UsersService.API.Middleware;
using UsersService.API.Middleware.Authentication;

namespace UsersService.API
{
    public static class DependencyInjection
    {
        public static void AddPresentation(this IServiceCollection services)
        {
            services.AddScoped<ExceptionMiddleware>();
            services.AddScoped<AuthorizationMiddleware>();

            services.AddScoped<IAuthorizationHandler, AuthorizationHandler>();

            services.AddGrpc();
        }

        public static void UsePresentation(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();

            app.MapGrpcService<Controllers.Grpc.AuthController>();
        }
    }
}
