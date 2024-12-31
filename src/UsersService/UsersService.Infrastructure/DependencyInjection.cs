using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Configuration;
using UsersService.Domain.Entities.SQL;
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

               InitializeSqlServerDatabaseAsync(configuration);
            });

            services.AddIdentityCore<UserEntity>()
                .AddRoles<RoleEntity>()
                .AddEntityFrameworkStores<UsersDbContext>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisTokens");
            });

            services.AddHangfire(configuration);

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<ICompaniesRepository, CompaniesRepository>();
            services.AddScoped<IResumesRepository, ResumesRepository>();
            services.AddScoped<ITokensRepository, TokensRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void UseInfrastructure(this WebApplication app)
        {
            app.UseHangfireDashboard();
        }

        private static void InitializeSqlServerDatabaseAsync(IConfiguration configuration)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("MasterDatabaseSqlServer"));

            connection.Open();

            using var command = new SqlCommand(configuration["Scripts:InitializeUsersDatabaseSqlServer"], connection);

            command.ExecuteNonQuery();
        }

        private static void InitializeHangfireDatabase(IConfiguration configuration)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("MasterDatabaseSqlServer"));

            connection.Open();

            using var command = new SqlCommand(configuration["Scripts:InitializeHangfireDatabaseSqlServer"], connection);

            command.ExecuteNonQuery();
        }

        internal static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            InitializeHangfireDatabase(configuration);

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

    }
}
