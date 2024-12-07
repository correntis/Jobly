using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using VacanciesService.Application.Abstractions;
using VacanciesService.Application.Services;
using VacanciesService.Application.Vacancies.Recommendations.Jobs;
using VacanciesService.Application.Vacancies.Recommendations.ML;

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
            services.AddScoped<IVacancyTrainingDataConverter, VacancyTrainingDataConverter>();

            services.AddSingleton<VacancyRecommendationsModel>();
        }

        public static void UseApplication(this WebApplication app)
        {
            ConfigureJobs();
        }

        public static void ConfigureJobs()
        {
            RecurringJob.AddOrUpdate<TrainVacancyRecommendationsJob>(
                "TrainVacancyRecommendationsJob",
                job => job.ExecuteAsync(),
                Cron.Daily);
        }
    }
}
