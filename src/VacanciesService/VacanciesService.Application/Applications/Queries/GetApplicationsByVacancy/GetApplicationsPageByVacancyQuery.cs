using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByVacancy
{
    public sealed record GetApplicationsPageByVacancyQuery(
        Guid VacancyId,
        int PageNumber,
        int PageSize) : IRequest<List<Domain.Models.Application>>;
}
