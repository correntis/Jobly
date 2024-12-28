using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetBestVacanciesForResumeQuery
{
    public sealed record GetBestVacanciesPageForResumeQuery(
        string ResumeId,
        int PageNumber,
        int PageSize) : IRequest<IEnumerable<Vacancy>>;
}
