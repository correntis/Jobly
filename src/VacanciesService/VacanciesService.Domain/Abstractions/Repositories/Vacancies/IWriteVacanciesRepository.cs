using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Domain.Abstractions.Repositories.Vacancies
{
    public interface IWriteVacanciesRepository : IWriteRepository
    {
        Task AddAsync(VacancyEntity vacancyEntity, CancellationToken token = default);
        void Delete(VacancyEntity vacancyEntity);
        void Update(VacancyEntity vacancyEntity);
    }
}