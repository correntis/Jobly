using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancyByCompanyQuery
{
    public sealed record GetVacanciesByCompanyQuery(Guid CompanyId) : IRequest<List<Vacancy>>;
}
