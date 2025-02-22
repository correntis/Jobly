using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Queries.GetUserForVacancyInteractions
{
    public sealed record GetUserForVacancyInteractionQuery(Guid UserId, Guid VacancyId) : IRequest<VacancyInteraction>;
}
