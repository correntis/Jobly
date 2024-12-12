using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using UsersService.Domain.Configuration;
using UsersService.Infrastructure.NoSQL;
using UsersService.Infrastructure.SQL;

namespace UsersService.Tests.Intergation
{
    public class IntergationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly IConfiguration _configuration;
        private readonly MsSqlContainer _sqlServerContainer;
        private readonly RedisContainer _redisContainer;
        private readonly MongoDbContainer _mongoDbContainer;

        public IntergationTestWebAppFactory()
        {
            _configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.Test.json")
               .Build();

            _sqlServerContainer = new MsSqlBuilder()
               .WithImage(_configuration["SqlServer:Image"])
               .WithPassword(_configuration["SqlServer:Password"])
               .WithEnvironment("SA_PASSWORD", "LZpb8R9Ozsrab6Vaez6L2oNsP1z79o2v")
               .WithEnvironment("ACCEPT_EULA", "Y")
               .WithPortBinding(
                   int.Parse(_configuration["SqlServer:HostPort"]),
                   int.Parse(_configuration["SqlServer:ContainerPort"]))
               .Build();

            _redisContainer = new RedisBuilder()
                .WithImage(_configuration["Redis:Image"])
                .WithPortBinding(
                    int.Parse(_configuration["Redis:HostPort"]),
                    int.Parse(_configuration["Redis:ContainerPort"]))
                .Build();

            _mongoDbContainer = new MongoDbBuilder()
                .WithImage(_configuration["MongoDb:Image"])
                .WithPortBinding(
                    int.Parse(_configuration["MongoDb:HostPort"]),
                    int.Parse(_configuration["MongoDb:ContainerPort"]))
                .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "root")
                .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "password")
                .WithEnvironment("MONGO_INITDB_DATABASE", "admin")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _sqlServerContainer?.StartAsync();
            await _redisContainer?.StartAsync();
            await _mongoDbContainer?.StartAsync();

            await InitializeSqlServerDatabaseAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _sqlServerContainer?.StopAsync();
            await _redisContainer?.StopAsync();
            await _mongoDbContainer?.StopAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                ConfigureTestUsersDbContext(services);
                ConfigureMongoDbContext(services);
            });
        }

        private void ConfigureMongoDbContext(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(MongoDbOptions));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.Configure<MongoDbOptions>(opts =>
            {
                opts.Url = _configuration.GetConnectionString("MongoDb");
                opts.Database = "TestDatabase";
            });

            services.AddSingleton<MongoDbContext>();
        }

        private void ConfigureTestUsersDbContext(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<UsersDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<UsersDbContext>(options =>
            {
                var connectionString = _configuration.GetConnectionString("UsersServiceDatabaseSqlServer");

                options.UseSqlServer(connectionString);
            });

            UseSqlServerMigrations(services);
        }

        private void UseSqlServerMigrations(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            dbContext.Database.Migrate();
        }

        private async Task InitializeSqlServerDatabaseAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("MasterDatabaseSqlServer"));

            await connection.OpenAsync();

            using var command = new SqlCommand(_configuration["Scripts:InitializeSqlServer"], connection);

            await command.ExecuteNonQueryAsync();
        }
    }
}
