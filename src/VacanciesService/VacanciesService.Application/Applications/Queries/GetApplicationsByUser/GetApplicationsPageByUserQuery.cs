using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByUser
{
    public sealed record GetApplicationsPageByUserQuery(
        Guid UserId,
        int PageNumber,
        int PageSize) : IRequest<List<Domain.Models.Application>>;
}
