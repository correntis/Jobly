using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Repositories.Write
{
    public class WriteVacanciesRepository : IWriteVacanciesRepository
    {
        private readonly IVacanciesWriteContext _vacanciesContext;

        public WriteVacanciesRepository(IVacanciesWriteContext vacanciesContext)
        {
            _vacanciesContext = vacanciesContext;
        }

        public async Task AddAsync(VacancyEntity vacancyEntity, CancellationToken token = default)
        {
            await _vacanciesContext.Vacancies.AddAsync(vacancyEntity, token);
        }

        public void Delete(VacancyEntity vacancyEntity)
        {
            _vacanciesContext.Vacancies.Remove(vacancyEntity);
        }

        public void Update(VacancyEntity vacancyEntity)
        {
            _vacanciesContext.Vacancies.Update(vacancyEntity);
        }

        public async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _vacanciesContext.SaveChangesAsync(token);
        }
    }
}
