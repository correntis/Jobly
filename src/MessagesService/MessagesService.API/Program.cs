using Elastic.Serilog.Sinks;
using MessagesService.Application;
using MessagesService.DataAccess;
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

services.AddApplication();
services.AddDataAccess(configuration);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
