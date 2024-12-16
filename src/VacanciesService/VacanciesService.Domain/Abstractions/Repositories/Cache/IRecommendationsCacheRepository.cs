using VacanciesService.Domain.Models;

namespace VacanciesService.Domain.Abstractions.Repositories.Cache
{
    public interface IRecommendationsCacheRepository
    {
        Task<List<Vacancy>> GetVacanciesAsync(string resumeId, CancellationToken token = default);
        Task RemoveVacanciesAsync(string resumeId, CancellationToken token = default);
        Task SetVacanciesAsync(string resumeId, List<Vacancy> vacancies, DateTime expires, CancellationToken token = default);
    }
}