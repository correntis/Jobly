using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByVacancyQuery
{
    public class GetApplicationsPageByVacancyQueryHandler
        : IRequestHandler<GetApplicationsPageByVacancyQuery, List<Domain.Models.Application>>
    {
        private readonly ILogger<GetApplicationsPageByVacancyQueryHandler> _logger;
        private readonly IReadApplicationsRepository _readApplicationsRepository;
        private readonly IMapper _mapper;

        public GetApplicationsPageByVacancyQueryHandler(
            ILogger<GetApplicationsPageByVacancyQueryHandler> logger,
            IReadApplicationsRepository readApplicationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readApplicationsRepository = readApplicationsRepository;
            _mapper = mapper;
        }

        public async Task<List<Domain.Models.Application>> Handle(GetApplicationsPageByVacancyQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for vacancy with ID {VacancyId}",
                request.GetType().Name,
                request.VacancyId);

            var applicationsEntities = await _readApplicationsRepository.GetPageByVacancyIncludeVacancy(
                request.VacancyId,
                request.PageNumber,
                request.PageSize,
                token);

            _logger.LogInformation(
                "Successfully handled {QueryName} for vacancy with ID {VacancyId}",
                request.GetType().Name,
                request.VacancyId);

            return _mapper.Map<List<Domain.Models.Application>>(applicationsEntities);
        }
    }
}
