using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories;

namespace VacanciesService.Application.Vacancies.Jobs
{
    public class DeleteVacancyIfSillArchivedJob
    {
        private readonly ILogger<DeleteVacancyIfSillArchivedJob> _logger;
        private readonly IVacanciesWriteContext _vacanciesContext;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public DeleteVacancyIfSillArchivedJob(
            ILogger<DeleteVacancyIfSillArchivedJob> logger,
            IVacanciesWriteContext vacanciesContext,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _vacanciesContext = vacanciesContext;
            _detailsRepository = detailsRepository;
        }

        public async Task ExecuteAsync(Guid vacancyId)
        {
            await DeleteVacancyApplicationsAsync(vacancyId);

            await DeleteVacancyAsync(vacancyId);

            await DeleteVacancyDetailsAsync(vacancyId);

            await _vacanciesContext.SaveChangesAsync();

            _logger.LogInformation("Vacancy with ID {VacancyId} has been deleted after being archived", vacancyId);
        }

        private async Task DeleteVacancyInteractions(Guid vacancyId)
        {
            var interactionsEntities = await _vacanciesContext.Interactions
                .Where(i => i.VacancyId == vacancyId)
                .ToListAsync();
        }

        private async Task DeleteVacancyDetailsAsync(Guid vacancyId)
        {
            var vacancyDetailsEntity = await _detailsRepository.GetByAsync(vd => vd.VacancyId, vacancyId);

            if (vacancyDetailsEntity is null)
            {
                _logger.LogInformation("VacancyDetails for vacancy with ID {VacancyId} not found, deletion skepped", vacancyId);
                return;
            }

            await _detailsRepository.DeleteByAsync(vd => vd.Id, vacancyDetailsEntity.Id);

            _logger.LogInformation("VacancyDetails for vacancy with ID {VacancyId} deleted", vacancyId);
        }

        private async Task DeleteVacancyAsync(Guid vacancyId)
        {
            var vacancyEntity = await _vacanciesContext.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancyId);

            if (vacancyEntity is null)
            {
                _logger.LogWarning("Vacancy with ID {VacancyId} not found for deletion", vacancyId);
                return;
            }

            if (!vacancyEntity.Archived)
            {
                _logger.LogInformation("Vacancy with ID {VacancyId} is no longer archived, deletion skipped", vacancyId);
                return;
            }

            _vacanciesContext.Vacancies.Remove(vacancyEntity);
        }

        private async Task DeleteVacancyApplicationsAsync(Guid vacancyId)
        {
            var applicationsEntities = await _vacanciesContext.Applications
                .Include(a => a.Vacancy)
                .Where(a => a.Vacancy.Id == vacancyId)
                .ToListAsync();

            _vacanciesContext.Applications.RemoveRange(applicationsEntities);
        }
    }
}
