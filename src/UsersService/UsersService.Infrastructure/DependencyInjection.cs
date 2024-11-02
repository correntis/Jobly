using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Domain.Configuration;
using UsersService.Infrastructure.NoSQL;
using UsersService.Infrastructure.SQL;

namespace UsersService.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrascructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));

            services.AddDbContext<UsersDbContext>(options =>
            {
               options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
            });

            services.AddSingleton<MongoDbContext>();
        }
    }
}
