namespace Jobly.Brokers.Abstractions
{
    public interface IBrokerProcuder
    {
        Task SendAsync<T>(T message, CancellationToken token = default) where T : class;
    }
}