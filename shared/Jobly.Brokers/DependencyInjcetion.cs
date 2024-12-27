using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Consumers;
using Jobly.Brokers.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Jobly.Brokers
{
    public static class DependencyInjcetion
    {
        public static void AddGlobalBrokers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new ConnectionFactory()
            {
                HostName = configuration["Broker:Host"],
                Port = int.Parse(configuration["Broker:Port"])
            });

            services.AddSingleton<IBrokerProcuder, BrokerProcuder>();

            services.AddSingleton(typeof(IBrokerConsumer<>), typeof(BrokerConsumer<>));
        }
    }
}
