using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;

namespace VacanciesService.Application.Vacancies.Queries.GetDistinctTechnologies
{
    public class GetDistinctTechnologiesQueryHandler : IRequestHandler<GetDistinctTechnologiesQuery, List<string>>
    {
        private readonly ILogger<GetDistinctTechnologiesQueryHandler> _logger;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public GetDistinctTechnologiesQueryHandler(
            ILogger<GetDistinctTechnologiesQueryHandler> logger,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _detailsRepository = detailsRepository;
        }

        public async Task<List<string>> Handle(GetDistinctTechnologiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start getting distinct technologies");

            var technologies = await _detailsRepository.GetDistinctTechnologiesAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} distinct technologies", technologies.Count);

            return technologies;
        }
    }
}

