using Microsoft.Extensions.DependencyInjection;
using VacanciesService.Application.Abstractions;
using VacanciesService.Application.Services;

namespace VacanciesService.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
            });

            services.AddAutoMapper(assembly);

            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
        }
    }
}
