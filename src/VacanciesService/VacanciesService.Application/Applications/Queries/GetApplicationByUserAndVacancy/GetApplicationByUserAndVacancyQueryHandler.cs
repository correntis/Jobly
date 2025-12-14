using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;

namespace VacanciesService.Application.Applications.Queries.GetApplicationByUserAndVacancy
{
    public class GetApplicationByUserAndVacancyQueryHandler
        : IRequestHandler<GetApplicationByUserAndVacancyQuery, Domain.Models.Application?>
    {
        private readonly ILogger<GetApplicationByUserAndVacancyQueryHandler> _logger;
        private readonly IReadApplicationsRepository _readApplicationsRepository;
        private readonly IMapper _mapper;

        public GetApplicationByUserAndVacancyQueryHandler(
            ILogger<GetApplicationByUserAndVacancyQueryHandler> logger,
            IReadApplicationsRepository readApplicationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readApplicationsRepository = readApplicationsRepository;
            _mapper = mapper;
        }

        public async Task<Domain.Models.Application?> Handle(GetApplicationByUserAndVacancyQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start getting application for user {UserId} and vacancy {VacancyId}",
                request.UserId,
                request.VacancyId);

            var applicationEntity = await _readApplicationsRepository.GetByUserAndVacancy(
                request.UserId,
                request.VacancyId,
                token);

            if (applicationEntity == null)
            {
                _logger.LogInformation(
                    "Application not found for user {UserId} and vacancy {VacancyId}",
                    request.UserId,
                    request.VacancyId);
                return null;
            }

            _logger.LogInformation(
                "Application found for user {UserId} and vacancy {VacancyId}",
                request.UserId,
                request.VacancyId);

            return _mapper.Map<Domain.Models.Application>(applicationEntity);
        }
    }
}

