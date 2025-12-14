using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsStatusCounts
{
    public sealed record GetApplicationsStatusCountsQuery(
        Guid UserId) : IRequest<ApplicationsStatusCounts>;
}

