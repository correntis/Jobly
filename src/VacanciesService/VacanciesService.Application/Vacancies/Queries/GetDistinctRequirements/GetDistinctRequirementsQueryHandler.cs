using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;

namespace VacanciesService.Application.Vacancies.Queries.GetDistinctRequirements
{
    public class GetDistinctRequirementsQueryHandler : IRequestHandler<GetDistinctRequirementsQuery, List<string>>
    {
        private readonly ILogger<GetDistinctRequirementsQueryHandler> _logger;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public GetDistinctRequirementsQueryHandler(
            ILogger<GetDistinctRequirementsQueryHandler> logger,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _detailsRepository = detailsRepository;
        }

        public async Task<List<string>> Handle(GetDistinctRequirementsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start getting distinct requirements");

            var requirements = await _detailsRepository.GetDistinctRequirementsAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} distinct requirements", requirements.Count);

            return requirements;
        }
    }
}

