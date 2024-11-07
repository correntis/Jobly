using Elastic.Serilog.Sinks;
using Serilog;
using UsersService.Application;
using UsersService.Infrastructure;
using UsersService.Presentation;

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

services.AddPresentation();
services.AddApplication(configuration);
services.AddInfrascructure(configuration);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UsePresentation();
app.UseApplication();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
