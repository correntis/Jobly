using MediatR;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand
{
    public sealed record AddVacancyDetailsCommand(
        Guid VacancyId,
        List<string> Requirements,
        List<string> Skills,
        List<string> Tags,
        List<string> Responsibilities,
        List<string> Benefits,
        List<string> Education,
        List<string> Technologies,
        List<Language> Languages,
        ExperienceLevel Experience,
        Salary Salary) : IRequest<string>;
}
