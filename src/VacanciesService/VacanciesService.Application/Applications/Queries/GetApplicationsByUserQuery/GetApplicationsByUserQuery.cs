using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByUserQuery
{
    public sealed record GetApplicationsByUserQuery(int UserId) : IRequest<List<Domain.Models.Application>>;
}
