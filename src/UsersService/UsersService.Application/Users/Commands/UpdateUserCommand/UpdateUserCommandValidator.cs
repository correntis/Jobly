using FluentValidation;
using UsersService.Domain.Constants;

namespace UsersService.Application.Users.Commands.UpdateUserCommand
{
    public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(c => c.FirstName)
                .NotNull()
                .NotEmpty()
                .WithMessage("First name is required");

            RuleFor(c => c.FirstName)
                .MaximumLength(BusinessRules.User.MaxFirstNameLength)
                .WithMessage("First name too long");

            RuleFor(c => c.LastName)
                .MaximumLength(BusinessRules.User.MaxLastNameLength)
                .WithMessage("Last name too long");
        }
    }
}
