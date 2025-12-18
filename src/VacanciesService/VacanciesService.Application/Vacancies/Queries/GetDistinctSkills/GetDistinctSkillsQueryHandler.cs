using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;

namespace VacanciesService.Application.Vacancies.Queries.GetDistinctSkills
{
    public class GetDistinctSkillsQueryHandler : IRequestHandler<GetDistinctSkillsQuery, List<string>>
    {
        private readonly ILogger<GetDistinctSkillsQueryHandler> _logger;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public GetDistinctSkillsQueryHandler(
            ILogger<GetDistinctSkillsQueryHandler> logger,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _detailsRepository = detailsRepository;
        }

        public async Task<List<string>> Handle(GetDistinctSkillsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start getting distinct skills");

            var skills = await _detailsRepository.GetDistinctSkillsAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} distinct skills", skills.Count);

            return skills;
        }
    }
}

