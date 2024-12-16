using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Domain.Abstractions.Repositories.Interactions
{
    public interface IWriteInteractionsRepository : IWriteRepository
    {
        Task AddAsync(VacancyInteractionEntity interactionEntity, CancellationToken token = default);
        void RemoveRange(List<VacancyInteractionEntity> interactionsEntities);
    }
}