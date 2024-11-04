using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Behaviors;

namespace UsersService.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddValidatorsFromAssembly(assembly);

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);

                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddAutoMapper(assembly);
        }
    }
}
