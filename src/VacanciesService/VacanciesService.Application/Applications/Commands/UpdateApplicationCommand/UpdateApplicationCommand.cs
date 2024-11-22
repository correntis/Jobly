using MediatR;

namespace VacanciesService.Application.Applications.Commands.UpdateApplicationCommand
{
    public sealed record UpdateApplicationCommand(
        int Id,
        string Status) : IRequest<int>;
}
