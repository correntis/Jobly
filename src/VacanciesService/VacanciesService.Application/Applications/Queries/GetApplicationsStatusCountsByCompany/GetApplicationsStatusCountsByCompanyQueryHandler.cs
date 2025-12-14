using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsStatusCountsByCompany
{
    public class GetApplicationsStatusCountsByCompanyQueryHandler
        : IRequestHandler<GetApplicationsStatusCountsByCompanyQuery, ApplicationsStatusCounts>
    {
        private readonly ILogger<GetApplicationsStatusCountsByCompanyQueryHandler> _logger;
        private readonly IReadApplicationsRepository _readApplicationsRepository;

        public GetApplicationsStatusCountsByCompanyQueryHandler(
            ILogger<GetApplicationsStatusCountsByCompanyQueryHandler> logger,
            IReadApplicationsRepository readApplicationsRepository)
        {
            _logger = logger;
            _readApplicationsRepository = readApplicationsRepository;
        }

        public async Task<ApplicationsStatusCounts> Handle(GetApplicationsStatusCountsByCompanyQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start getting application status counts for company {CompanyId}",
                request.CompanyId);

            var counts = await _readApplicationsRepository.GetStatusCountsByCompany(request.CompanyId, token);

            _logger.LogInformation(
                "Successfully got application status counts for company {CompanyId}",
                request.CompanyId);

            return new ApplicationsStatusCounts
            {
                Total = counts.Total,
                Unread = counts.Unread,
                Accepted = counts.Accepted,
                Rejected = counts.Rejected
            };
        }
    }
}

