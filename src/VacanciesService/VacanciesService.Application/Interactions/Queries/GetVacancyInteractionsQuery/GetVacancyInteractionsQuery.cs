using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Queries.GetVacancyInteractionsQuery
{
    public sealed record GetVacancyInteractionsQuery(Guid VacancyId) : IRequest<List<VacancyInteraction>>;
}
