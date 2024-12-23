using MessagesService.Presentation.Hubs;
using MessagesService.Presentation.Hubs.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace MessagesService.Presentation
{
    public static class DependencyInjection
    {
        public static void AddPresentation(this IServiceCollection services)
        {
            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }

        public static void MapPresentation(this WebApplication app)
        {
            app.MapHub<MessagesHub>("/messagesHub");
        }
    }
}
