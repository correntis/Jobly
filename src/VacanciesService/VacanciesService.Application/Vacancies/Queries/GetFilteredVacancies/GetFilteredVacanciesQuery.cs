using MediatR;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetFilteredVacancies
{
    public sealed record GetFilteredVacanciesQuery(VacancyDetailsFilter Filter, Guid? UserId = null) : IRequest<List<Vacancy>>;
}
