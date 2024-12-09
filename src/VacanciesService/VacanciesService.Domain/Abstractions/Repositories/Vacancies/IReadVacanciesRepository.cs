using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Domain.Abstractions.Repositories.Vacancies
{
    public interface IReadVacanciesRepository
    {
        Task<VacancyEntity> GetAsync(Guid id, CancellationToken token = default);
        Task<List<VacancyEntity>> GetAllByCompanyAsync(Guid id, CancellationToken token = default);
        Task<List<VacancyEntity>> GetAllIn(List<Guid> ids, CancellationToken token = default);
        Task LoadApplications(VacancyEntity vacancyEntity, CancellationToken token = default);
    }
}