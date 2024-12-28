using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Application.Vacancies.Jobs;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Vacancies.Commands.ArchiveVacancy
{
    public class ArchiveVacancyCommandHandler : IRequestHandler<ArchiveVacancyCommand, Guid>
    {
        private readonly ILogger<ArchiveVacancyCommandHandler> _logger;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IWriteVacanciesRepository _writeVacanciesRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ArchiveVacancyCommandHandler(
            ILogger<ArchiveVacancyCommandHandler> logger,
            IReadVacanciesRepository readVacanciesRepository,
            IWriteVacanciesRepository writeVacanciesRepository,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _readVacanciesRepository = readVacanciesRepository;
            _writeVacanciesRepository = writeVacanciesRepository;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Guid> Handle(ArchiveVacancyCommand request, CancellationToken token = default)
        {
            _logger.LogInformation(
                 "Start handling {CommandName} for vacancy with ID {VacancyId}",
                 request.GetType().Name,
                 request.Id);

            var vacancyEntity = await _readVacanciesRepository.GetAsync(request.Id, token);

            if(vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {request.Id} not found");
            }

            vacancyEntity.Archived = true;

            _writeVacanciesRepository.Update(vacancyEntity);

            await _writeVacanciesRepository.SaveChangesAsync(token);

            CreateDeletionJob(vacancyEntity.Id);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy ID {VacancyId}",
                request.GetType().Name,
                vacancyEntity.Id);

            return vacancyEntity.Id;
        }

        private void CreateDeletionJob(Guid vacancyId)
        {
            _backgroundJobClient.Schedule<DeleteVacancyIfSillArchivedJob>(
                j => j.ExecuteAsync(vacancyId),
                BusinessRules.Vacancy.DeletionAfterArchiveTime);
        }
    }
}
