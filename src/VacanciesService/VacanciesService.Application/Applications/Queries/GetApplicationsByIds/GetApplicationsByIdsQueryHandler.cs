using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByIds
{
    public class GetApplicationsByIdsQueryHandler : IRequestHandler<GetApplicationsByIdsQuery, List<Domain.Models.Application>>
    {
        private readonly ILogger<GetApplicationsByIdsQueryHandler> _logger;
        private readonly IReadApplicationsRepository _readApplicationsRepository;
        private readonly IMapper _mapper;

        public GetApplicationsByIdsQueryHandler(
            ILogger<GetApplicationsByIdsQueryHandler> logger,
            IReadApplicationsRepository readApplicationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readApplicationsRepository = readApplicationsRepository;
            _mapper = mapper;
        }

        public async Task<List<Domain.Models.Application>> Handle(GetApplicationsByIdsQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start {QueryName} for ids {ApplicationsIds}", request.GetType().Name, request.ApplicationsIds);

            var applicationsEntities = await _readApplicationsRepository.GetByIdsIncludeVacancy(request.ApplicationsIds,token);

            _logger.LogInformation("Success {QueryName} for ids {ApplicationsIds}", request.GetType().Name, request.ApplicationsIds);

            return _mapper.Map<List<Domain.Models.Application>>(applicationsEntities);
        }
    }
}
