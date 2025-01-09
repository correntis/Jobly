using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacanciesByCompany
{
    public sealed record GetVacanciesByCompanyQuery(Guid CompanyId) : IRequest<List<Vacancy>>;
}
