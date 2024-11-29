using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Vacancies.Commands.ArchiveVacancyCommand
{
    public class ArchiveVacancyCommandHandler : IRequestHandler<ArchiveVacancyCommand, Guid>
    {
        private readonly ILogger<ArchiveVacancyCommandHandler> _logger;
        private readonly IVacanciesWriteContext _vacanciesContext;

        public ArchiveVacancyCommandHandler(
            ILogger<ArchiveVacancyCommandHandler> logger,
            IVacanciesWriteContext vacanciesContext)
        {
            _logger = logger;
            _vacanciesContext = vacanciesContext;
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

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy ID {VacancyId}",
                request.GetType().Name,
                vacancyEntity.Id);

            return vacancyEntity.Id;
        }
    }
}
