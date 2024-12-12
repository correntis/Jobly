using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories;

namespace VacanciesService.Application.Vacancies.Commands.DeleteVacancyCommand
{
    public class DeleteVacancyCommandHandler : IRequestHandler<DeleteVacancyCommand, int>
    {
        private readonly ILogger<DeleteVacancyCommandHandler> _logger;
        private readonly IVacanciesWriteContext _vacanciesContext;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public DeleteVacancyCommandHandler(
            ILogger<DeleteVacancyCommandHandler> logger,
            IVacanciesWriteContext vacanciesContext,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _vacanciesContext = vacanciesContext;
            _detailsRepository = detailsRepository;
        }

        public async Task<int> Handle(DeleteVacancyCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for vacancy with ID {VacancyId}",
                request.GetType().Name,
                request.Id);

            var vacancyEntity = await _vacanciesContext.Vacancies
                .FirstOrDefaultAsync(v => v.Id == request.Id, token);

            await _vacanciesContext.Vacancies
                .Entry(vacancyEntity)
                .Collection(v => v.Applications)
                .LoadAsync(token);

            _vacanciesContext.Vacancies.Remove(vacancyEntity);

            await _vacanciesContext.SaveChangesAsync(token);

            await _detailsRepository.DeleteByAsync(vd => vd.VacancyId, vacancyEntity.Id, token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy ID {VacancyId}",
                request.GetType().Name,
                vacancyEntity.Id);

            return vacancyEntity.Id;
        }
    }
}
