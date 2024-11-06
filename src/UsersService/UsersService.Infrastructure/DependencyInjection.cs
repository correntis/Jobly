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
        public static void AddInfrascructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));

            services.AddSingleton<MongoDbContext>();

            services.AddDbContext<UsersDbContext>(options =>
            {
               options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisTokens");
            });

            services.AddScoped<ICompaniesRepository, CompaniesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IResumesRepository, ResumesRepository>();
            services.AddScoped<ITokensRepository, TokensRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
