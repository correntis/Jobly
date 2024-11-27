using MediatR;

namespace VacanciesService.Application.Vacancies.Commands.DeleteVacancyCommand
{
    public sealed record DeleteVacancyCommand(int Id) : IRequest<int>;
}
