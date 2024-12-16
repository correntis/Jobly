using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Configuration.Options;
using MessagesService.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessagesService.DataAccess
{
    public static class DependencyInjection
    {
        public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));

            services.AddSingleton<MongoContext>();

            services.AddScoped<INotificationsRepository, NotificationsRepository>();
            services.AddScoped<IMessagesRepository, MessagesRepository>();
        }
    }
}
