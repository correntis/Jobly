using MediatR;

namespace VacanciesService.Application.Vacancies.Commands.AddVacancyCommand
{
    public sealed record AddVacancyCommand(
        string Title,
        string EmploymentType,
        Guid CompanyId,
        DateTime DeadlineAt) : IRequest<Guid>;
}
