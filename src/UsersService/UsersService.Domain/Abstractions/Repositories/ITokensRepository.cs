namespace UsersService.Domain.Abstractions.Repositories
{
    public interface ITokensRepository
    {
        Task<string> GetAsync(string key, CancellationToken cancellationToken);
        Task RemoveAsync(string key, CancellationToken cancellationToken);
        Task SetAsync(string key, string value, DateTime expiresTime, CancellationToken cancellationToken);
    }
}