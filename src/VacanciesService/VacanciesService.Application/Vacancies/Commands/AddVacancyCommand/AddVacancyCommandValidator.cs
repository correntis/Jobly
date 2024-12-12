using FluentValidation;
using VacanciesService.Domain.Constants;

namespace VacanciesService.Application.Vacancies.Commands.AddVacancyCommand
{
    public class AddVacancyCommandValidator : AbstractValidator<AddVacancyCommand>
    {
        public AddVacancyCommandValidator()
        {
            RuleFor(v => v.Title)
                .NotNull()
                .NotEmpty()
                .WithMessage("Title can not be null or empty string");

            RuleFor(v => v.Title)
                .MaximumLength(BusinessRules.Vacancy.TitleMaxLength)
                .WithMessage($"Title must be shorter then {BusinessRules.Vacancy.TitleMaxLength} characters");

            RuleFor(v => v.EmploymentType)
                .MaximumLength(BusinessRules.Vacancy.EmploymentTypeMaxLenght)
                .WithMessage($"Employment type must be shorter then {BusinessRules.Vacancy.EmploymentTypeMaxLenght}");

            RuleFor(v => v.DeadlineAt)
                .Must(deadline => DateTime.UtcNow < deadline)
                .WithMessage("Deadline must be more then current date");

            RuleFor(v => v.CompanyId)
                .NotNull()
                .NotEqual(Guid.Empty)
                .WithMessage("CompanyId can not be null or empty");
        }
    }
}
