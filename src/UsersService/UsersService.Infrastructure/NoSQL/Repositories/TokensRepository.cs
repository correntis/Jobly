using Microsoft.Extensions.Caching.Distributed;
using UsersService.Domain.Abstractions.Repositories;

namespace UsersService.Infrastructure.NoSQL.Repositories
{
    public class TokensRepository : ITokensRepository
    {
        private readonly IDistributedCache _cache;

        public TokensRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetAsync(string key, string value, DateTime expiresTime, CancellationToken cancellationToken)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(expiresTime);

            await _cache.SetStringAsync(key, value, options, cancellationToken);
        }

        public async Task<string> GetAsync(string key, CancellationToken cancellationToken)
        {
            return await _cache.GetStringAsync(key, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
