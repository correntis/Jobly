using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VacanciesService.Presentation.Abstractions;
using VacanciesService.Presentation.Middleware;
using VacanciesService.Presentation.Middleware.Authorization;

namespace VacanciesService.Presentation
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
        }
    }
}
