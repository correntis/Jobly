using Elastic.Serilog.Sinks;
using Jobly.Brokers;
using Serilog;
using VacanciesService.Application;
using VacanciesService.Infrastructure;
using VacanciesService.Presentation;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

if(builder.Environment.IsProduction())
{
    configuration
    .AddJsonFile("appsettings.Container.json");
}

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console()
        .WriteTo.Elasticsearch([new Uri(context.Configuration["Elastic:Uri"])])
        .ReadFrom.Configuration(context.Configuration);
});

services.AddApplication();
services.AddPresentation();
services.AddInfrastructure(configuration, builder.Environment);

services.AddGlobalBrokers(configuration);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(options =>
{
    options
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UsePresentation();
app.UseInfrastructure();
app.UseApplication();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.Run();

public partial class Program
{
}