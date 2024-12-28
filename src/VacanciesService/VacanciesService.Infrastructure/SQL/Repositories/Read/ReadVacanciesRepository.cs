using Microsoft.EntityFrameworkCore;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Repositories.Read
{
    public class ReadVacanciesRepository : IReadVacanciesRepository
    {
        private readonly IVacanciesReadContext _vacanciesContext;

        public ReadVacanciesRepository(IVacanciesReadContext vacanciesContext)
        {
            _vacanciesContext = vacanciesContext;
        }

        public async Task<VacancyEntity> GetAsync(Guid id, CancellationToken token = default)
        {
            return await _vacanciesContext.Vacancies.FirstOrDefaultAsync(v => v.Id == id, token);
        }

        public async Task<List<VacancyEntity>> GetAllByCompanyAsync(Guid id, CancellationToken token = default)
        {
            return await _vacanciesContext.Vacancies
                .Where(v => v.CompanyId == id)
                .ToListAsync(token);
        }

        public async Task<List<VacancyEntity>> GetAllIn(List<Guid> ids, CancellationToken token = default)
        {
            return await _vacanciesContext.Vacancies
                .Where(v => ids.Contains(v.Id))
                .ToListAsync(token);
        }

        public async Task LoadApplications(VacancyEntity vacancyEntity, CancellationToken token = default)
        {
            await _vacanciesContext.Vacancies
                .Entry(vacancyEntity)
                .Collection(v => v.Applications)
                .LoadAsync(token);
        }
    }
}
