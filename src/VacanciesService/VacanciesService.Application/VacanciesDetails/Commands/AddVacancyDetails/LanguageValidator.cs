using FluentValidation;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetails
{
    public class LanguageValidator : AbstractValidator<Language>
    {
        public LanguageValidator()
        {
            RuleFor(lang => lang.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("Language name cannot be empty or null");

            RuleFor(lang => lang.Level)
                .NotEmpty()
                .NotNull()
                .WithMessage("Language name cannot be empty or null");
        }
    }
}
