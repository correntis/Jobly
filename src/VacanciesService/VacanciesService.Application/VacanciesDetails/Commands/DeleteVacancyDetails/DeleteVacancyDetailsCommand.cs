using MediatR;

namespace VacanciesService.Application.VacanciesDetails.Commands.DeleteVacancyDetails
{
    public sealed record DeleteVacancyDetailsCommand(string Id) : IRequest<string>;
}
