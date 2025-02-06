using Elastic.Serilog.Sinks;
using MessagesService.Application;
using MessagesService.DataAccess;
using MessagesService.Presentation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console()
        .WriteTo.Elasticsearch([new Uri(context.Configuration["Elastic:Uri"])])
        .ReadFrom.Configuration(context.Configuration);
});

services.AddDataAccess(configuration);
services.AddApplication();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddPresentation(configuration);

var app = builder.Build();

app.UseCors(options =>
{
    options
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.UsePresentation();

app.Run();
