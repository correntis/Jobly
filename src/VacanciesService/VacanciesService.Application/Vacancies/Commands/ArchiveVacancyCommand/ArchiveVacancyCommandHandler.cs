using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Application.Vacancies.Jobs;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Vacancies.Commands.ArchiveVacancyCommand
{
    public class ArchiveVacancyCommandHandler : IRequestHandler<ArchiveVacancyCommand, Guid>
    {
        private readonly ILogger<ArchiveVacancyCommandHandler> _logger;
        private readonly IVacanciesWriteContext _vacanciesContext;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ArchiveVacancyCommandHandler(
            ILogger<ArchiveVacancyCommandHandler> logger,
            IVacanciesWriteContext vacanciesContext,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _vacanciesContext = vacanciesContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Guid> Handle(ArchiveVacancyCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                 "Start handling {CommandName} for vacancy with ID {VacancyId}",
                 request.GetType().Name,
                 request.Id);

            var vacancyEntity = await _vacanciesContext.Vacancies.FirstOrDefaultAsync(v => v.Id == request.Id, token);

            if (vacancyEntity == null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {request.Id} not found");
            }

            vacancyEntity.Archived = true;

            await _vacanciesContext.SaveChangesAsync(token);

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
