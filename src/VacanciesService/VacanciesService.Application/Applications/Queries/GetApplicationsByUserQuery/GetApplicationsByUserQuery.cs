using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByUserQuery
{
    public sealed record GetApplicationsByUserQuery(Guid UserId) : IRequest<List<Domain.Models.Application>>;
}
