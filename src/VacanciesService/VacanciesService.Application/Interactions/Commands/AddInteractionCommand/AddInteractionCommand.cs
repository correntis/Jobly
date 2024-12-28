using MediatR;
using VacanciesService.Domain.Enums;

namespace VacanciesService.Application.Interactions.Commands.AddInteractionCommand
{
    public sealed record AddInteractionCommand(
        Guid UserId,
        Guid VacancyId,
        InteractionType Type) : IRequest<Guid>;
}
