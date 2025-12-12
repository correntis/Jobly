using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Enums;

namespace VacanciesService.Domain.Abstractions.Repositories.Interactions
{
    public interface IReadInteractionsRepository
    {
        Task<List<VacancyInteractionEntity>> GetAllByVacancyAsync(Guid vacancyId, CancellationToken token = default);
        Task<List<VacancyInteractionEntity>> GetLikedByVacanciesAsync(List<Guid> vacanciesIds, CancellationToken token = default);
        Task<List<VacancyInteractionEntity>> GetAllByUserAsync(Guid userId, CancellationToken token = default);
        Task<List<VacancyInteractionEntity>> GetAllByUserAndVacanciesAsync(Guid userId, List<Guid> vacanciesIds, CancellationToken token = default);
        Task<VacancyInteractionEntity> GetByUserAndVacancy(Guid userId, Guid vacancyId, CancellationToken token = default);
        Task<List<Guid>> GetDislikedVacancyIdsByUserAsync(Guid userId, List<Guid> vacanciesIds, CancellationToken token = default);
        Task<List<Guid>> GetVacancyIdsByUserAndTypeAsync(Guid userId, InteractionType interactionType, int pageNumber, int pageSize, CancellationToken token = default);
    }
}