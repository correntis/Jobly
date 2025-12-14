using MediatR;

namespace VacanciesService.Application.Applications.Queries.GetApplicationByUserAndVacancy
{
    public sealed record GetApplicationByUserAndVacancyQuery(
        Guid UserId,
        Guid VacancyId) : IRequest<Domain.Models.Application?>;
}

