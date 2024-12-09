using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByUserQuery
{
    public class GetApplicationsPageByUserQueryHandler
        : IRequestHandler<GetApplicationsPageByUserQuery, List<Domain.Models.Application>>
    {
        private readonly ILogger<GetApplicationsPageByUserQueryHandler> _logger;
        private readonly IReadApplicationsRepository _readApplicationsRepository;
        private readonly IMapper _mapper;

        public GetApplicationsPageByUserQueryHandler(
            ILogger<GetApplicationsPageByUserQueryHandler> logger,
            IReadApplicationsRepository readApplicationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readApplicationsRepository = readApplicationsRepository;
            _mapper = mapper;
        }

        public async Task<List<Domain.Models.Application>> Handle(GetApplicationsPageByUserQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for user with ID {UserId}",
                request.GetType().Name,
                request.UserId);

            var applicationsEntities = await _readApplicationsRepository.GetPageByUserIncludeVacancy(
                request.UserId,
                request.PageNumber,
                request.PageSize,
                token);

            _logger.LogInformation(
                "Successfully handled {QueryName} for user with ID {UserId}",
                request.GetType().Name,
                request.UserId);

            return _mapper.Map<List<Domain.Models.Application>>(applicationsEntities);
        }
    }
}
