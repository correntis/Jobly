using MediatR;

namespace VacanciesService.Application.Interactions.Commands.AddInteractionCommand
{
    public sealed record AddInteractionCommand(
        Guid UserId,
        Guid VacancyId,
        int Type) : IRequest<Guid>;
}
