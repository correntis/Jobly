using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancyQuery
{
    public sealed record GetVacancyQuery(int Id) : IRequest<Vacancy>;
}
