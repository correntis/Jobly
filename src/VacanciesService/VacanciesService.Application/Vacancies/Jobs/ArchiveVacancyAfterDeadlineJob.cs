using Hangfire;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Vacancies.Jobs
{
    public class ArchiveVacancyAfterDeadlineJob
    {
        private readonly ILogger<ArchiveVacancyAfterDeadlineJob> _logger;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IWriteVacanciesRepository _writeVacanciesRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ArchiveVacancyAfterDeadlineJob(
            ILogger<ArchiveVacancyAfterDeadlineJob> logger,
            IReadVacanciesRepository readVacanciesRepository,
            IWriteVacanciesRepository writeVacanciesRepository,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _readVacanciesRepository = readVacanciesRepository;
            _writeVacanciesRepository = writeVacanciesRepository;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task ExecuteAsync(Guid vacancyId)
        {
            var vacancyEntity = await _readVacanciesRepository.GetAsync(vacancyId);

            if(vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {vacancyId} not found");
            }

            vacancyEntity.Archived = true;

            _writeVacanciesRepository.Update(vacancyEntity);

            await _writeVacanciesRepository.SaveChangesAsync();

            _logger.LogInformation("Vacancy with id {VacancyId} successfully archived on deadline", vacancyId);
        }
    }
}
