using FluentValidation;

namespace VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetails
{
    public class AddVacancyDetailsCommandValidator : AbstractValidator<AddVacancyDetailsCommand>
    {
        public AddVacancyDetailsCommandValidator()
        {
            RuleFor(vd => vd.VacancyId)
                .NotEmpty()
                .WithMessage("VacancyId cannot be empty or default GUID");

            RuleForEach(vd => vd.Requirements)
                .NotEmpty()
                .WithMessage("Requirements cannot contain empty values");

            RuleForEach(vd => vd.Skills)
                .NotEmpty()
                .WithMessage("Skills cannot contain empty values");

            RuleForEach(vd => vd.Tags)
                .NotEmpty()
                .WithMessage("Tags cannot contain empty values");

            RuleForEach(vd => vd.Responsibilities)
                .NotEmpty()
                .WithMessage("Responsibilities cannot contain empty values");

            RuleForEach(vd => vd.Benefits)
                .NotEmpty()
                .WithMessage("Benefits cannot contain empty values");

            RuleForEach(vd => vd.Education)
                .NotEmpty()
                .WithMessage("Education cannot contain empty values");

            RuleForEach(vd => vd.Technologies)
                .NotEmpty()
                .WithMessage("Technologies cannot contain empty values");

            RuleForEach(vd => vd.Languages)
                .SetValidator(new LanguageValidator())
                .When(vd => vd.Languages is not null, ApplyConditionTo.CurrentValidator);

            RuleFor(vd => vd.Experience)
                .SetValidator(new ExperienceValidator())
                .When(vd => vd.Experience is not null, ApplyConditionTo.CurrentValidator);

            RuleFor(vd => vd.Salary)
                .NotNull()
                .SetValidator(new SalaryValidator())
                .When(vd => vd.Salary is not null, ApplyConditionTo.CurrentValidator);
        }
    }
}
