using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByIds
{
    public sealed record GetApplicationsByIdsQuery(List<Guid> ApplicationsIds) : IRequest<List<Domain.Models.Application>>;
}
