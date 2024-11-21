using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Configuration;
using VacanciesService.Infrastructure.API.Services;
using VacanciesService.Infrastructure.NoSQL;
using VacanciesService.Infrastructure.NoSQL.Repositories;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
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

            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            services.AddScoped<IVacanciesReadContext, VacanciesReadContext>();
            services.AddScoped<IVacanciesWriteContext, VacanciesWriteContext>();
            services.AddScoped<IVacanciesDetailsRepository, VacanciesDetailsRepository>();

            services.AddScoped<ICurrencyApiService, CurrencyApiService>();
        }
    }
}
