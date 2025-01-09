using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancy
{
    public sealed record GetVacancyQuery(Guid Id) : IRequest<Vacancy>;
}
