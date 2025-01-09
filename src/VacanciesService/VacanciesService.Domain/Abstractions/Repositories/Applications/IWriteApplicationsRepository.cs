using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Domain.Abstractions.Repositories.Applications
{
    public interface IWriteApplicationsRepository : IWriteRepository
    {
        Task AddAsync(ApplicationEntity applicationEntity, CancellationToken token = default);
        void Update(ApplicationEntity applicationEntity);
        void RemoveRange(List<ApplicationEntity> applicationEntities);
        void AttachVacancy(VacancyEntity vacancyEntity);
    }
}