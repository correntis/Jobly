using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;

namespace VacanciesService.Application.Vacancies.Jobs
{
    public class DeleteVacancyIfSillArchivedJob
    {
        private readonly ILogger<DeleteVacancyIfSillArchivedJob> _logger;
        private readonly IVacanciesDetailsRepository _detailsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IWriteVacanciesRepository _writeVacanciesRepository;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IWriteInteractionsRepository _writeInteractionsRepository;
        private readonly IReadApplicationsRepository _readApplicationsRepository;
        private readonly IWriteApplicationsRepository _writeApplicationsRepository;

        public DeleteVacancyIfSillArchivedJob(
            ILogger<DeleteVacancyIfSillArchivedJob> logger,
            IVacanciesDetailsRepository detailsRepository,
            IReadVacanciesRepository readVacanciesRepository,
            IWriteVacanciesRepository writeVacanciesRepository,
            IReadInteractionsRepository readInteractionsRepository,
            IWriteInteractionsRepository writeInteractionsRepository,
            IReadApplicationsRepository readApplicationsRepository,
            IWriteApplicationsRepository writeApplicationsRepository)
        {
            _logger = logger;
            _detailsRepository = detailsRepository;
            _readVacanciesRepository = readVacanciesRepository;
            _writeVacanciesRepository = writeVacanciesRepository;
            _readInteractionsRepository = readInteractionsRepository;
            _writeInteractionsRepository = writeInteractionsRepository;
            _readApplicationsRepository = readApplicationsRepository;
            _writeApplicationsRepository = writeApplicationsRepository;
        }

        public async Task ExecuteAsync(Guid vacancyId)
        {
            await DeleteVacancyApplicationsAsync(vacancyId);

            await DeleteVacancyAsync(vacancyId);

            await DeleteVacancyDetailsAsync(vacancyId);

            await _writeVacanciesRepository.SaveChangesAsync();

            _logger.LogInformation("Vacancy with ID {VacancyId} has been deleted after being archived", vacancyId);
        }

        private async Task DeleteVacancyInteractions(Guid vacancyId)
        {
            var interactionsEntities = await _readInteractionsRepository.GetAllByVacancy(vacancyId, CancellationToken.None);

            _writeInteractionsRepository.RemoveRange(interactionsEntities);
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
            var vacancyEntity = await _readVacanciesRepository.GetAsync(vacancyId);

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

            _writeVacanciesRepository.Delete(vacancyEntity);
        }

        private async Task DeleteVacancyApplicationsAsync(Guid vacancyId)
        {
            var applicationsEntities = await _readApplicationsRepository.GetAllByVacancyAsync(vacancyId, CancellationToken.None);

            _writeApplicationsRepository.RemoveRange(applicationsEntities);
        }
    }
}
