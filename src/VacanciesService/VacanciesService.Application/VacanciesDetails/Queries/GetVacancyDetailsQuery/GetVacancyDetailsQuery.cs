using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Queries.GetVacancyDetailsQuery
{
    public sealed record GetVacancyDetailsQuery(int VacancyId) : IRequest<VacancyDetails>;
}
