using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using VacanciesService.Domain.Abstractions.Repositories.Cache;
using VacanciesService.Domain.Models;

namespace VacanciesService.Infrastructure.NoSQL.Repositories
{
    public class RecommendationsCacheRepository : IRecommendationsCacheRepository
    {
        private readonly IDistributedCache _cache;

        private readonly string _baseKey = "ResumeId:";

        public RecommendationsCacheRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetVacanciesAsync(
            string resumeId,
            List<Vacancy> vacancies,
            DateTime expires,
            CancellationToken token = default)
        {
            var key = _baseKey + resumeId;
            var value = JsonSerializer.Serialize(vacancies);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(expires);

            await _cache.SetStringAsync(key, value, options, token);
        }

        public async Task<List<Vacancy>> GetVacanciesAsync(string resumeId, CancellationToken token = default)
        {
            var key = _baseKey + resumeId;

            var cacheString = await _cache.GetStringAsync(key, token);

            if (string.IsNullOrEmpty(cacheString))
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<Vacancy>>(cacheString);
        }

        public async Task RemoveVacanciesAsync(string resumeId, CancellationToken token = default)
        {
            var key = _baseKey + resumeId;

            await _cache.RemoveAsync(key, token);
        }
    }
}
