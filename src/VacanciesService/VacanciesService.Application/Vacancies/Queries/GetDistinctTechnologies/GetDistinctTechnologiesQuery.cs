using MediatR;

namespace VacanciesService.Application.Vacancies.Queries.GetDistinctTechnologies
{
    public sealed record GetDistinctTechnologiesQuery() : IRequest<List<string>>;
}

