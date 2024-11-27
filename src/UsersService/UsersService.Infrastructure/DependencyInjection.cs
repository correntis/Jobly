using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Configuration;
using UsersService.Infrastructure.NoSQL;
using UsersService.Infrastructure.NoSQL.Repositories;
using UsersService.Infrastructure.SQL;
using UsersService.Infrastructure.SQL.Repositories;

namespace UsersService.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));

            services.AddSingleton<MongoDbContext>();

            services.AddDbContext<UsersDbContext>(options =>
            {
               options.UseSqlServer(configuration.GetConnectionString("UsersDatabaseSqlServer"));
            });

            services.AddStackExchangeRedisCache(async options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisTokens");

                await InitializeSqlServerDatabaseAsync(configuration);
            });

            services.AddScoped<ICompaniesRepository, CompaniesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IResumesRepository, ResumesRepository>();
            services.AddScoped<ITokensRepository, TokensRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static async Task InitializeSqlServerDatabaseAsync(IConfiguration configuration)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("MasterDatabaseSqlServer"));

            await connection.OpenAsync();

            using var command = new SqlCommand(configuration["Scripts:InitializeUsersDatabaseSqlServer"], connection);

            await command.ExecuteNonQueryAsync();
        }
    }
}
