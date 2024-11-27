using MediatR;

namespace VacanciesService.Application.Applications.Commands.AddApplicationCommand
{
    public sealed record AddApplicationCommand(
        int UserId,
        int VacancyId) : IRequest<int>;
}
