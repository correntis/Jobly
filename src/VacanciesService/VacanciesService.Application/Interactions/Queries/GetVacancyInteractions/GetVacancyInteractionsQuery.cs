using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Queries.GetVacancyInteractions
{
    public sealed record GetVacancyInteractionsQuery(Guid VacancyId) : IRequest<List<VacancyInteraction>>;
}
