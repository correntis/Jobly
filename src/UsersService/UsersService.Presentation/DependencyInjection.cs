using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Presentation.Abstractions;
using UsersService.Presentation.Middleware;
using UsersService.Presentation.Middleware.Authentication;

namespace UsersService.Presentation
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
            app.MapGrpcService<Controllers.Grpc.UsersServiceController>();
        }
    }
}
