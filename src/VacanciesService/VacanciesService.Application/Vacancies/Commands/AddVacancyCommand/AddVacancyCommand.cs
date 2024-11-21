using MediatR;

namespace VacanciesService.Application.Vacancies.Commands.AddVacancyCommand
{
    public sealed record AddVacancyCommand(
        string Title,
        string EmploymentType,
        int CompanyId,
        DateTime DeadlineAt) : IRequest<int>;
}
