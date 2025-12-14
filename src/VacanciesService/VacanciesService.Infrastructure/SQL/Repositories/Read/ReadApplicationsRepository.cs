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
            return await _vacanciesContext.Applications
                .Include(app => app.Vacancy)
                .FirstOrDefaultAsync(a => a.Id == id, token);
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

        public async Task<List<ApplicationEntity>> GetByIdsIncludeVacancy(List<Guid> ids, CancellationToken token = default)
        {
            return await _vacanciesContext.Applications
                .Include(a => a.Vacancy)
                .Where(a => ids.Contains(a.Id))
                .ToListAsync(token);
        }

        public async Task<bool> ExistForUserAndVacancy(Guid userId, Guid vacancyId, CancellationToken token = default)
        {
            return await _vacanciesContext.Applications
               .Where(a => a.UserId == userId && a.Vacancy.Id == vacancyId)
               .AnyAsync(token);
        }

        public async Task<ApplicationEntity?> GetByUserAndVacancy(Guid userId, Guid vacancyId, CancellationToken token = default)
        {
            return await _vacanciesContext.Applications
                .Include(a => a.Vacancy)
                .Where(a => a.UserId == userId && a.Vacancy.Id == vacancyId)
                .FirstOrDefaultAsync(token);
        }

        public async Task<Domain.Models.ApplicationsStatusCounts> GetStatusCountsByUser(Guid userId, CancellationToken token = default)
        {
            var applications = await _vacanciesContext.Applications
                .Where(a => a.UserId == userId)
                .ToListAsync(token);

            return new Domain.Models.ApplicationsStatusCounts
            {
                Total = applications.Count,
                Unread = applications.Count(a => a.Status == "Unread"),
                Accepted = applications.Count(a => a.Status == "Accepted"),
                Rejected = applications.Count(a => a.Status == "Rejected")
            };
        }

        public async Task<Domain.Models.ApplicationsStatusCounts> GetStatusCountsByCompany(Guid companyId, CancellationToken token = default)
        {
            var applications = await _vacanciesContext.Applications
                .Include(a => a.Vacancy)
                .Where(a => a.Vacancy.CompanyId == companyId)
                .ToListAsync(token);

            return new Domain.Models.ApplicationsStatusCounts
            {
                Total = applications.Count,
                Unread = applications.Count(a => a.Status == "Unread"),
                Accepted = applications.Count(a => a.Status == "Accepted"),
                Rejected = applications.Count(a => a.Status == "Rejected")
            };
        }
    }
}
