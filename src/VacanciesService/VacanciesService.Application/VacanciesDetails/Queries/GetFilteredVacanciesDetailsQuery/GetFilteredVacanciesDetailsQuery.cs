using MediatR;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Queries.GetFilteredVacanciesDetailsQuery
{
    public sealed record GetFilteredVacanciesDetailsQuery(VacancyDetailsFilter Filter) : IRequest<List<VacancyDetails>>;
}
