using MediatR;

namespace VacanciesService.Application.Applications.Commands.UpdateApplicationCommand
{
    public sealed record UpdateApplicationCommand(
        Guid Id,
        string Status) : IRequest<Guid>;
}
