using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Repositories.Write
{
    public class WriteApplicationsRepository : IWriteApplicationsRepository
    {
        private readonly IVacanciesWriteContext _vacanciesContext;

        public WriteApplicationsRepository(IVacanciesWriteContext vacanciesContext)
        {
            _vacanciesContext = vacanciesContext;
        }

        public async Task AddAsync(ApplicationEntity applicationEntity, CancellationToken token = default)
        {
            await _vacanciesContext.Applications.AddAsync(applicationEntity, token);
        }

        public void Update(ApplicationEntity applicationEntity)
        {
            _vacanciesContext.Applications.Update(applicationEntity);
        }

        public void RemoveRange(List<ApplicationEntity> applicationEntities)
        {
            _vacanciesContext.Applications.RemoveRange(applicationEntities);
        }

        public async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _vacanciesContext.SaveChangesAsync(token);
        }

        public void AttachVacancy(VacancyEntity vacancyEntity)
        {
            _vacanciesContext.Vacancies.Attach(vacancyEntity);
        }
    }
}
