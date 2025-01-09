using MediatR;

namespace VacanciesService.Application.Vacancies.Commands.ArchiveVacancy
{
    public sealed record ArchiveVacancyCommand(Guid Id) : IRequest<Guid>;
}
