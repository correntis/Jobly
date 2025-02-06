using Jobly.Brokers;
using MessagesService.Presentation.Abstractions;
using MessagesService.Presentation.HostedServices;
using MessagesService.Presentation.Hubs;
using MessagesService.Presentation.Hubs.Providers;
using MessagesService.Presentation.Middleware.Authorization;
using MessagesService.Presentation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessagesService.Presentation
{
    public static class DependencyInjection
    {
        public static void AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AuthorizationMiddleware>();
            services.AddScoped<IAuthorizationHandler, AuthorizationHandler>();
            services.AddGrpc();

            services.AddSignalR()
                .AddHubOptions<MessagesHub>(options =>
                {
                    options.EnableDetailedErrors = true;
                });

            services.AddSingleton<NotificationsService>();
            services.AddSingleton<ChatsService>();

            services.AddSingleton<IUserIdProvider, UserIdProvider>();

            services.AddGlobalBrokers(configuration);

            services.AddHostedServices();
        }

        public static void UsePresentation(this WebApplication app)
        {
            app.UseMiddleware<AuthorizationMiddleware>();

            app.MapHub<MessagesHub>("hubs/messages");
            app.MapHub<NotificationsHub>("hubs/notifications");
            app.MapHub<ChatsHub>("hubs/chats");
        }

        private static void AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<UserRegistrationService>();
            services.AddHostedService<ApplicationResponseService>();
            services.AddHostedService<VacancyDeadlineService>();
            services.AddHostedService<RecomendVacancyService>();
            services.AddHostedService<ResumeViewService>();
            services.AddHostedService<VacancyApplicationService>();
        }
    }
}
