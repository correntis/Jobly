using MediatR;

namespace VacanciesService.Application.Applications.Commands.AddApplicationCommand
{
    public sealed record AddApplicationCommand(
        Guid UserId,
        Guid VacancyId) : IRequest<Guid>;
}
