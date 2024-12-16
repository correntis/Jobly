using MediatR;

namespace VacanciesService.Application.VacanciesDetails.Commands.DeleteVacancyDetailsCommand
{
    public sealed record DeleteVacancyDetailsCommand(string Id) : IRequest<string>;
}
