using FluentValidation;
using System.Text;
using UsersService.Domain.Constants;

namespace UsersService.Application.Users.Commands.AddUserCommand
{
    public sealed class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(c => c.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage("Email is required");

            RuleFor(c => c.Email)
                .EmailAddress()
                .WithMessage("Incorrect email");

            RuleFor(c => c.Email)
                .MaximumLength(BusinessRules.User.MaxEmailLength)
                .WithMessage("Email is too long");

            RuleFor(c => c.Password)
                .NotEmpty()
                .NotNull()
                .WithMessage("Password is required");

            RuleFor(c => c.Password)
                .MinimumLength(BusinessRules.User.MinPasswordLength)
                .WithMessage("Password is too short");

            RuleFor(c => c.Type)
                .NotNull()
                .NotEmpty()
                .WithMessage("Type is required");

            RuleFor(c => c.Type)
                .Must(t => BusinessRules.User.Types.Contains(t))
                .WithMessage(GetTypeValidationMessage());

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

        private string GetTypeValidationMessage()
        {
            var message = new StringBuilder("Type must be");

            foreach (var type in BusinessRules.User.Types)
            {
                message.Append($" {type},");
            }

            message.Remove(message.Length - 1, 1);

            return message.ToString();
        }
    }
}
