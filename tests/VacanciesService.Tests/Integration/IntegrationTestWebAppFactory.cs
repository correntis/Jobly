using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Configuration;
using VacanciesService.Infrastructure.NoSQL;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Tests.Integration
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly IConfiguration _configuration;
        private readonly PostgreSqlContainer _postgreContainer;
        private readonly MsSqlContainer _sqlServerContainer;
        private readonly MongoDbContainer _mongoContainer;
        private readonly RedisContainer _redisContainer;

        public Mock<IUsersService> UsersServiceMock { get; private set; }

        public IntegrationTestWebAppFactory()
        {
            _configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.Test.json")
               .Build();

            _sqlServerContainer = new MsSqlBuilder()
               .WithImage(_configuration["SqlServer:Image"])
               .WithPassword(_configuration["SqlServer:Password"])
               .WithEnvironment("SA_PASSWORD", _configuration["SqlServer:Password"])
               .WithEnvironment("ACCEPT_EULA", "Y")
               .WithPortBinding(
                   int.Parse(_configuration["SqlServer:HostPort"]),
                   int.Parse(_configuration["SqlServer:ContainerPort"]))
               .Build();

            _postgreContainer = new PostgreSqlBuilder()
                .WithImage(_configuration["PostgreSql:Image"])
                .WithEnvironment("POSTGRES_USER", _configuration["PostgreSql:User"])
                .WithEnvironment("POSTGRES_DB", _configuration["PostgreSql:Database"])
                .WithEnvironment("POSTGRES_PASSWORD", _configuration["PostgreSql:Password"])
                .WithPortBinding(
                    int.Parse(_configuration["PostgreSql:HostPort"]),
                    int.Parse(_configuration["PostgreSql:ContainerPort"]))
                .Build();

            _redisContainer = new RedisBuilder()
                .WithImage(_configuration["Redis:Image"])
                .WithPortBinding(
                    int.Parse(_configuration["Redis:HostPort"]),
                    int.Parse(_configuration["Redis:ContainerPort"]))
                .Build();

            _mongoContainer = new MongoDbBuilder()
                .WithImage(_configuration["MongoDb:Image"])
                .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", _configuration["MongoDb:Username"])
                .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", _configuration["MongoDb:Password"])
                .WithEnvironment("MONGO_INITDB_DATABASE", _configuration["MongoDb:Database"])
                .WithPortBinding(
                    int.Parse(_configuration["MongoDb:HostPort"]),
                    int.Parse(_configuration["MongoDb:ContainerPort"]))
                .Build();

            UsersServiceMock = new Mock<IUsersService>();
        }

        public async Task InitializeAsync()
        {
            await _postgreContainer.StartAsync();
            await _mongoContainer?.StartAsync();
            await _redisContainer?.StartAsync();
            await _sqlServerContainer?.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgreContainer.StopAsync();
            await _mongoContainer?.StopAsync();
            await _redisContainer?.StopAsync();
            await _sqlServerContainer?.StopAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                ConfigureUsersServiceMock(services);
                ConfigureTestPostgreSqlDbContext(services);
                ConfigureMongoDbContext(services);
            });
        }

        private void ConfigureUsersServiceMock(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUsersService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton(UsersServiceMock.Object);
        }

        private void ConfigureMongoDbContext(IServiceCollection services)
        {
            RemoveService(services, typeof(MongoDbOptions));

            services.Configure<MongoDbOptions>(opts =>
            {
                opts.Url = _configuration.GetConnectionString("MongoDb");
                opts.Database = "TestDatabase";
            });

            services.AddSingleton<MongoDbContext>();
        }

        private void UsePostgreSqlMigrations(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();
            dbContext.Database.Migrate();
        }

        private void ConfigureTestPostgreSqlDbContext(IServiceCollection services)
        {
            RemoveService(services, typeof(IVacanciesWriteContext));
            RemoveService(services, typeof(VacanciesWriteContext));
            RemoveService(services, typeof(DbContextOptions<VacanciesWriteContext>));
            RemoveService(services, typeof(IVacanciesReadContext));
            RemoveService(services, typeof(VacanciesReadContext));
            RemoveService(services, typeof(DbContextOptions<VacanciesReadContext>));
            RemoveService(services, typeof(DbContextOptions));
            RemoveService(services, typeof(DbContextOptions));

            var connectionString = _configuration.GetConnectionString("PostgreSql");

            services.AddDbContext<VacanciesWriteContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddDbContext<VacanciesReadContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IVacanciesWriteContext, VacanciesWriteContext>();
            services.AddScoped<IVacanciesReadContext, VacanciesReadContext>();

            UsePostgreSqlMigrations(services);
        }

        private void RemoveService(IServiceCollection services, Type serviceType)
        {
            var descriptor = services.FirstOrDefault(s => s.ServiceType == serviceType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
        }
    }
}
