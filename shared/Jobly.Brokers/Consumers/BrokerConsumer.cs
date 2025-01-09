using Jobly.Brokers.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Jobly.Brokers.Consumers
{
    public class BrokerConsumer<TQueue> : BrokerBase, IBrokerConsumer<TQueue>
    {
        private readonly AsyncEventingBasicConsumer _consumer;

        public BrokerConsumer(ConnectionFactory factory)
            : base(factory)
        {
            _consumer = new AsyncEventingBasicConsumer(Channel);
        }

        public async void AddListener(AsyncEventHandler<BasicDeliverEventArgs> handler)
        {
            _consumer.ReceivedAsync += handler;

            await CreateQueueAsync<TQueue>();

            await BasicConsumeAsync<TQueue>(_consumer);
        }
    }
}
