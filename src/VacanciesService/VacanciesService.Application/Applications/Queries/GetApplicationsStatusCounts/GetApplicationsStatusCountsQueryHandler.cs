using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsStatusCounts
{
    public class GetApplicationsStatusCountsQueryHandler
        : IRequestHandler<GetApplicationsStatusCountsQuery, ApplicationsStatusCounts>
    {
        private readonly ILogger<GetApplicationsStatusCountsQueryHandler> _logger;
        private readonly IReadApplicationsRepository _readApplicationsRepository;

        public GetApplicationsStatusCountsQueryHandler(
            ILogger<GetApplicationsStatusCountsQueryHandler> logger,
            IReadApplicationsRepository readApplicationsRepository)
        {
            _logger = logger;
            _readApplicationsRepository = readApplicationsRepository;
        }

        public async Task<ApplicationsStatusCounts> Handle(GetApplicationsStatusCountsQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start getting application status counts for user {UserId}",
                request.UserId);

            var counts = await _readApplicationsRepository.GetStatusCountsByUser(request.UserId, token);

            _logger.LogInformation(
                "Successfully got application status counts for user {UserId}",
                request.UserId);

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

