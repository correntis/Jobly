using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Domain.Abstractions.Repositories.Interactions
{
    public interface IReadInteractionsRepository
    {
        Task<List<VacancyInteractionEntity>> GetAllByVacancy(Guid vacancyId, CancellationToken token = default);
        Task<List<VacancyInteractionEntity>> GetAllByUser(Guid userId, CancellationToken token = default);
        Task<List<VacancyInteractionEntity>> GetAllByUserAndVacancies(Guid userId, List<Guid> vacanciesIds, CancellationToken token = default);
    }
}