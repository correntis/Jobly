namespace UsersService.Domain.Abstractions.Repositories
{
    public interface ITokensRepository
    {
        Task<string> GetAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task SetAsync(string key, string value, DateTime expiresTime, CancellationToken cancellationToken = default);
    }
}