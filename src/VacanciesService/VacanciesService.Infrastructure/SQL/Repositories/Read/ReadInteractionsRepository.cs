using Microsoft.EntityFrameworkCore;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Repositories.Read
{
    public class ReadInteractionsRepository : IReadInteractionsRepository
    {
        private readonly IVacanciesReadContext _vacanciesContext;

        public ReadInteractionsRepository(IVacanciesReadContext vacanciesContext)
        {
            _vacanciesContext = vacanciesContext;
        }

        public async Task<List<VacancyInteractionEntity>> GetAllByVacancy(Guid vacancyId, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.VacancyId == vacancyId)
                .ToListAsync();
        }

        public async Task<List<VacancyInteractionEntity>> GetAllByUser(Guid userId, CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<VacancyInteractionEntity>> GetAllByUserAndVacancies(
            Guid userId,
            List<Guid> vacanciesIds,
            CancellationToken token = default)
        {
            return await _vacanciesContext.Interactions
                .Where(i => vacanciesIds.Contains(i.VacancyId) && i.UserId == userId)
                .ToListAsync(token);
        }
    }
}
