using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByVacancyQuery
{
    public sealed record GetApplicationsByVacancyQuery(Guid VacancyId) : IRequest<List<Domain.Models.Application>>;
}
