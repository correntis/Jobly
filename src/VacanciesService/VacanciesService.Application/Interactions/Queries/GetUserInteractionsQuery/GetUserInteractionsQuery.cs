using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Queries.GetUserInteractionsQuery
{
    public sealed record GetUserInteractionsQuery(Guid UserId) : IRequest<List<VacancyInteraction>>;
}
