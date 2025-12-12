using Microsoft.EntityFrameworkCore;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Enums;

namespace VacanciesService.Infrastructure.SQL.Repositories.Read
{
    public class ReadInteractionsRepository : IReadInteractionsRepository
    {
        private readonly IVacanciesReadContext _vacanciesContext;

        public ReadInteractionsRepository(IVacanciesReadContext vacanciesContext)
        {
            _vacanciesContext = vacanciesContext;
        }

        public async Task<List<VacancyInteractionEntity>> GetAllByVacancyAsync(Guid vacancyId, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.VacancyId == vacancyId)
                .ToListAsync();
        }

        public async Task<List<VacancyInteractionEntity>> GetLikedByVacanciesAsync(List<Guid> vacanciesIds, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.Type == (int)InteractionType.Like)
                .Where(i => vacanciesIds.Contains(i.VacancyId))
                .ToListAsync(token);
        }

        public async Task<List<VacancyInteractionEntity>> GetAllByUserAsync(Guid userId, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<VacancyInteractionEntity>> GetAllByUserAndVacanciesAsync(
            Guid userId,
            List<Guid> vacanciesIds,
            CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => vacanciesIds.Contains(i.VacancyId) && i.UserId == userId)
                .ToListAsync(token);
        }

        public async Task<VacancyInteractionEntity> GetByUserAndVacancy(Guid userId, Guid vacancyId, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.UserId == userId && i.VacancyId == vacancyId)
                .FirstOrDefaultAsync(token);
        }

        public async Task<List<Guid>> GetDislikedVacancyIdsByUserAsync(Guid userId, List<Guid> vacanciesIds, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.UserId == userId 
                    && vacanciesIds.Contains(i.VacancyId) 
                    && i.Type == (int)InteractionType.Dislike)
                .Select(i => i.VacancyId)
                .ToListAsync(token);
        }

        public async Task<List<Guid>> GetVacancyIdsByUserAndTypeAsync(Guid userId, InteractionType interactionType, int pageNumber, int pageSize, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.UserId == userId && i.Type == (int)interactionType)
                .OrderByDescending(i => i.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => i.VacancyId)
                .ToListAsync(token);
        }
    }
}
