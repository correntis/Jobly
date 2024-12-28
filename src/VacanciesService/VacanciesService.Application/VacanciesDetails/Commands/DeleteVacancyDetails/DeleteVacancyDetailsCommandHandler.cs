using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.VacanciesDetails.Commands.DeleteVacancyDetails
{
    public class DeleteVacancyDetailsCommandHandler : IRequestHandler<DeleteVacancyDetailsCommand, string>
    {
        private readonly ILogger<DeleteVacancyDetailsCommandHandler> _logger;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public DeleteVacancyDetailsCommandHandler(
            ILogger<DeleteVacancyDetailsCommandHandler> logger,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _detailsRepository = detailsRepository;
        }

        public async Task<string> Handle(DeleteVacancyDetailsCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for vacancy_details with ID {VacancyId}",
                request.GetType().Name,
                request.Id);

            var detailsEntity = await _detailsRepository.GetByAsync(vd => vd.Id, request.Id, token);
            if(detailsEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy_details with ID {request.Id} not found");
            }

            await _detailsRepository.DeleteByAsync(vd => vd.Id, request.Id, token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy_details with ID {VacancyId}",
                request.GetType().Name,
                request.Id);

            return request.Id;
        }
    }
}
