using MediatR;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetFilteredVacanciesQuery
{
    public sealed record GetFilteredVacanciesQuery(VacancyDetailsFilter Filter) : IRequest<List<Vacancy>>;
}
