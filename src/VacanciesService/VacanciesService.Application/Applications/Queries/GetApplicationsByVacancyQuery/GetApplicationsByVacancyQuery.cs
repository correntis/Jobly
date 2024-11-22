using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByVacancyQuery
{
    public sealed record GetApplicationsByVacancyQuery(int VacancyId) : IRequest<List<Domain.Models.Application>>;
}
