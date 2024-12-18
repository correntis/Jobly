using MediatR;

namespace VacanciesService.Application.Vacancies.Commands.DeleteVacancyCommand
{
    public sealed record DeleteVacancyCommand(Guid Id) : IRequest<Guid>;
}
