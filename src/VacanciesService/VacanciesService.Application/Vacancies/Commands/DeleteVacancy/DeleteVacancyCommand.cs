using MediatR;

namespace VacanciesService.Application.Vacancies.Commands.DeleteVacancy
{
    public sealed record DeleteVacancyCommand(Guid Id) : IRequest<Guid>;
}
