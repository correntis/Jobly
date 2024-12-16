using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByUserQuery
{
    public sealed record GetApplicationsPageByUserQuery(
        Guid UserId,
        int PageNumber,
        int PageSize) : IRequest<List<Domain.Models.Application>>;
}
