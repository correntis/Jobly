using MediatR;

namespace VacanciesService.Application.Applications.Commands.UpdateApplication
{
    public sealed record UpdateApplicationCommand(
        Guid Id,
        string Status) : IRequest<Guid>;
}
