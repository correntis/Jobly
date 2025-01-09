using RabbitMQ.Client.Events;

namespace Jobly.Brokers.Abstractions
{
    public interface IBrokerConsumer<TQueue>
    {
        void AddListener(AsyncEventHandler<BasicDeliverEventArgs> handler);
    }
}