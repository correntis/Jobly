using Jobly.Brokers.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Jobly.Brokers.Producers
{
    public class BrokerProcuder : BrokerBase, IBrokerProcuder
    {
        private readonly ILogger<BrokerProcuder> _logger;

        public BrokerProcuder(ILogger<BrokerProcuder> logger, ConnectionFactory factory)
            : base(factory)
        {
            _logger = logger;
        }

        public async Task SendAsync<TQueue>(TQueue message, CancellationToken token = default) where TQueue : class
        {
            _logger.LogInformation(
                "[Broker] Start produce message {@Message} for queue {QueueName}",
                typeof(TQueue).Name,
                message);

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await CreateQueueAsync<TQueue>(token);

            await BasicPublishAsync<TQueue>(messageBody, token);

            _logger.LogInformation(
                "[Broker] Successfully produced message for queue {QueueName}",
                message);
        }
    }
}
