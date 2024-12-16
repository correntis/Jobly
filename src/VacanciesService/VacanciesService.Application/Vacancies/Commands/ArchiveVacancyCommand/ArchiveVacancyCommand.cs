using MediatR;

namespace VacanciesService.Application.Vacancies.Commands.ArchiveVacancyCommand
{
    public sealed record ArchiveVacancyCommand(Guid Id) : IRequest<Guid>;
}
