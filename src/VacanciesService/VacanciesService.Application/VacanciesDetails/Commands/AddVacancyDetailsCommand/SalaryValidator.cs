using FluentValidation;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand
{
    public class SalaryValidator : AbstractValidator<Salary>
    {
        public SalaryValidator()
        {
            RuleFor(s => s.Min)
                .GreaterThanOrEqualTo(0)
                .When(s => s.Min is not null, ApplyConditionTo.CurrentValidator)
                .WithMessage("Minimum salary must be greater than or equal to 0");

            RuleFor(s => s.Max)
                .GreaterThanOrEqualTo(s => s.Min)
                .When(s => s.Max is not null && s.Min is not null, ApplyConditionTo.CurrentValidator)
                .WithMessage("Maximum salary must be greater than or equal to minimum salary");
        }
    }
}
