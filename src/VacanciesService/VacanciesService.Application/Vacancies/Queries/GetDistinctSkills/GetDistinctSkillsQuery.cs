using MediatR;

namespace VacanciesService.Application.Vacancies.Queries.GetDistinctSkills
{
    public sealed record GetDistinctSkillsQuery() : IRequest<List<string>>;
}

