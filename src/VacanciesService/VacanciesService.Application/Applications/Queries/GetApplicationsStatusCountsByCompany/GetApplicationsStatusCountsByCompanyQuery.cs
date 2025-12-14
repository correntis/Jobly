using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsStatusCountsByCompany
{
    public sealed record GetApplicationsStatusCountsByCompanyQuery(
        Guid CompanyId) : IRequest<ApplicationsStatusCounts>;
}

