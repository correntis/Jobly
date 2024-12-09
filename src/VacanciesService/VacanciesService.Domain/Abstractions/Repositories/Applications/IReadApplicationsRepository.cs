using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Domain.Abstractions.Repositories.Applications
{
    public interface IReadApplicationsRepository
    {
        Task<ApplicationEntity> GetAsync(Guid id, CancellationToken token = default);
        Task<List<ApplicationEntity>> GetAllByVacancyAsync(Guid vacancyId, CancellationToken token = default);
        Task<List<ApplicationEntity>> GetPageByUserIncludeVacancy(Guid userId, int pageNumber, int pageSize, CancellationToken token = default);
        Task<List<ApplicationEntity>> GetPageByVacancyIncludeVacancy(Guid vacancyId, int pageNumber, int pageSize, CancellationToken token = default);

    }
}