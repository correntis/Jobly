using Hangfire;
using Jobly.Protobufs.Authorization.Client;
using Jobly.Protobufs.Users.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Abstractions.Repositories.Cache;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Configuration;
using VacanciesService.Infrastructure.API.Services;
using VacanciesService.Infrastructure.Grpc;
using VacanciesService.Infrastructure.NoSQL;
using VacanciesService.Infrastructure.NoSQL.Repositories;
using VacanciesService.Infrastructure.SQL;
using VacanciesService.Infrastructure.SQL.Repositories.Read;
using VacanciesService.Infrastructure.SQL.Repositories.Write;

namespace VacanciesService.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddAutoMapper(assembly);

            services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));
            services.Configure<CurrencyApiOptions>(configuration.GetSection(nameof(CurrencyApiOptions)));

            services.AddDbContext<VacanciesReadContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ReadPostgreDatabase"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddDbContext<VacanciesWriteContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("WritePostgreDatabase"));
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisVacanciesCache");
            });

            services.AddHangfire(configuration);

            services.AddGrpcClients(configuration);

            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            services.AddScoped<IVacanciesReadContext, VacanciesReadContext>();
            services.AddScoped<IVacanciesWriteContext, VacanciesWriteContext>();

            services.AddScoped<IVacanciesDetailsRepository, VacanciesDetailsRepository>();
            services.AddScoped<IRecommendationsCacheRepository, RecommendationsCacheRepository>();
            services.AddScoped<IReadVacanciesRepository, ReadVacanciesRepository>();
            services.AddScoped<IReadInteractionsRepository, ReadInteractionsRepository>();
            services.AddScoped<IReadApplicationsRepository, ReadApplicationsRepository>();
            services.AddScoped<IWriteVacanciesRepository, WriteVacanciesRepository>();
            services.AddScoped<IWriteInteractionsRepository, WriteInteractionsRepository>();
            services.AddScoped<IWriteApplicationsRepository, WriteApplicationsRepository>();

            services.AddScoped<ICurrencyApiService, CurrencyApiServiceDevelopmentMock>();
            //services.AddScoped<ICurrencyApiService, CurrencyApiService>();
        }

        public static void UseInfrastructure(this WebApplication app)
        {
            app.UseHangfireDashboard();
        }

        internal static void AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IUsersService, Grpc.UsersService>();

            services
                .AddGrpcClient<AuthorizationGrpcService.AuthorizationGrpcServiceClient>(options =>
                {
                    options.Address = new Uri(configuration["Jobly:UsersService"]);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                });

            services
                .AddGrpcClient<UsersGrpcService.UsersGrpcServiceClient>(options =>
                {
                    options.Address = new Uri(configuration["Jobly:UsersService"]);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                });
        }

        internal static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            InitializeSqlServerDatabaseAsync(configuration);

            services.AddHangfire(options =>
            {
                options
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(configuration.GetConnectionString("HangfireDatabaseSqlServer"));
            });

            services.AddHangfireServer();
        }

        private static void InitializeSqlServerDatabaseAsync(IConfiguration configuration)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("MasterDatabaseSqlServer"));

            connection.Open();

            using var command = new SqlCommand(configuration["Scripts:InitializeHangfireDatabaseSqlServer"], connection);

            command.ExecuteNonQuery();
        }
    }
}
