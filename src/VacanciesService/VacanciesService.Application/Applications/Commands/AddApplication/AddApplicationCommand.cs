using MediatR;

namespace VacanciesService.Application.Applications.Commands.AddApplication
{
    public sealed record AddApplicationCommand(
        Guid UserId,
        Guid VacancyId) : IRequest<Guid>;
}
