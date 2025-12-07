using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace UsersService.Infrastructure;

public static class MigrationExtension
{
    public static void MigrateDbContext<TContext>(
        this WebApplication app,
        Action<TContext, IServiceProvider>? seed = null)
        where TContext : DbContext
    {
        const int maxRetriesNumber = 10;

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

        try
        {
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(
                    maxRetriesNumber,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retry, _) =>
                    {
                        logger.LogWarning(
                            exception,
                            "Retry #{Retry}/{MaxRetries} after {Delay} sec \r\n Error: {ExceptionType} \r\n Message: {Message}",
                            retry,
                            maxRetriesNumber,
                            timeSpan.TotalSeconds,
                            exception.GetType().Name,
                            exception.Message);
                    });

            retryPolicy.Execute(() =>
            {
                context.Database.Migrate();
                seed?.Invoke(context, services);
            });

            logger.LogInformation(
                "Successfully migrated database associated with context {DbContextName}",
                typeof(TContext).Name);
        }
        catch(Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to migrate database for context {DbContextName} after {MaxRetries} attempts",
                typeof(TContext).Name,
                maxRetriesNumber);

            throw;
        }
    }
}
