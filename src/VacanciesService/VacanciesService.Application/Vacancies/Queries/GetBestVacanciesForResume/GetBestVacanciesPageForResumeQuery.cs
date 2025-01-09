using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetBestVacanciesForResume
{
    public sealed record GetBestVacanciesPageForResumeQuery(
        string ResumeId,
        int PageNumber,
        int PageSize) : IRequest<IEnumerable<Vacancy>>;
}
