using Jobly.Protobufs.Authorization.Client;
using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Configuration.Options;
using MessagesService.DataAccess.Grpc;
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

            services.AddSingleton<INotificationsRepository, NotificationsRepository>();
            services.AddSingleton<IMessagesRepository, MessagesRepository>();
            services.AddSingleton<IChatsRepository, ChatsRepository>();
            services.AddSingleton<ITemplatesRepository, TemplatesRepository>();

            AddGrpcClients(services, configuration);

            SeedMongoDb(services);
        }

        internal static void AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthorizationService, AuthorizationService>();

            services
                .AddGrpcClient<AuthorizationGrpcService.AuthorizationGrpcServiceClient>(options =>
                {
                    options.Address = new Uri(configuration["Jobly:UsersService"]);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                });
        }

        internal static void SeedMongoDb(IServiceCollection services)
        {
            using(var scope = services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MongoContext>();

                context.SeedTemplates();
            }
        }
    }
}
