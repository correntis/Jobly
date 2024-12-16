using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using UsersService.Application.Behaviors;
using UsersService.Application.Services;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Configuration;
using UsersService.Domain.Constants;

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
            services.AddScoped<IImagesService, ImagesService>();
        }

        public static void UseApplication(this WebApplication app)
        {
            app.UseStaticImages();
        }

        public static void UseStaticImages(this WebApplication app)
        {
            var imagesDirectory = Path.Combine(app.Environment.ContentRootPath, BusinessRules.Image.Folder);

            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(imagesDirectory),
                RequestPath = "/resources",
            });
        }
    }
}
