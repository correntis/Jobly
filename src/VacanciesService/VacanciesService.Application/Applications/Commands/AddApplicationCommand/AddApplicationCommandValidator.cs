using FluentValidation;

namespace VacanciesService.Application.Applications.Commands.AddApplicationCommand
{
    class AddApplicationCommandValidator : AbstractValidator<AddApplicationCommand>
    {
        public AddApplicationCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.VacancyId)
                .NotEmpty()
                .WithMessage("Vacancy ID is required.");
        }
    }
}
