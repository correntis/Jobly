using Microsoft.EntityFrameworkCore;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Repositories.Read
{
    public class ReadApplicationsRepository : IReadApplicationsRepository
    {
        private readonly IVacanciesReadContext _vacanciesContext;

        public ReadApplicationsRepository(IVacanciesReadContext vacanciesContext)
        {
            _vacanciesContext = vacanciesContext;
        }

        public async Task<List<ApplicationEntity>> GetAllByVacancyAsync(Guid vacancyId, CancellationToken token = default)
        {
            return await _vacanciesContext.Applications
                .Where(a => a.Vacancy.Id == vacancyId)
                .ToListAsync();
        }

        public async Task<ApplicationEntity> GetAsync(Guid id, CancellationToken token = default)
        {
            return await _vacanciesContext.Applications.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<ApplicationEntity>> GetPageByUserIncludeVacancy(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken token = default)
        {
            return await _vacanciesContext.Applications
                .Include(a => a.Vacancy)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public async Task<List<ApplicationEntity>> GetPageByVacancyIncludeVacancy(
            Guid vacancyId,
            int pageNumber,
            int pageSize,
            CancellationToken token = default)
        {
            return await _vacanciesContext.Applications
                .Include(a => a.Vacancy)
                .Where(a => a.Vacancy.Id == vacancyId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }
    }
}
