using MediatR;

namespace VacanciesService.Application.Vacancies.Queries.GetDistinctRequirements
{
    public sealed record GetDistinctRequirementsQuery() : IRequest<List<string>>;
}

