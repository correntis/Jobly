using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Behaviors;
using UsersService.Application.Services;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Configuration;

namespace UsersService.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

            services.AddValidatorsFromAssembly(assembly);

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);

                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddAutoMapper(assembly);

            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<ITokensService, TokensService>();
        }
    }
}
