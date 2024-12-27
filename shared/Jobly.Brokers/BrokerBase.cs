using RabbitMQ.Client;

namespace Jobly.Brokers
{
    public abstract class BrokerBase
    {
        private readonly IConnection _connection;
        protected readonly IChannel Channel;

        protected BrokerBase(ConnectionFactory factory)
        {
            _connection = factory.CreateConnectionAsync().Result;
            Channel = _connection.CreateChannelAsync().Result;
        }

        protected async Task CreateQueueAsync<TQueue>(CancellationToken token = default)
        {
            await Channel.QueueDeclareAsync(
                queue: typeof(TQueue).Name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: token);
        }

        protected async Task BasicPublishAsync<TQueue>(byte[] messageBody, CancellationToken token = default  )
        {
            var properties = new BasicProperties()
            {
                ContentType = "application/json",
                Persistent = true
            };

            await Channel.BasicPublishAsync(
                exchange: "",
                routingKey: typeof(TQueue).Name,
                mandatory: true,
                basicProperties: properties,
                body: messageBody,
                cancellationToken: token);
        }

        protected async Task BasicConsumeAsync<TQueue>(IAsyncBasicConsumer consumer, CancellationToken token = default)
        {
            await Channel.BasicConsumeAsync(
                queue: typeof(TQueue).Name,
                autoAck: true,
                consumer: consumer,
                noLocal: false,
                exclusive: false,
                consumerTag: Guid.NewGuid().ToString(),
                arguments: null,
                cancellationToken: token);
        }

        ~BrokerBase()
        {
            _connection?.Dispose();
            Channel?.Dispose();
        }
    }
}
