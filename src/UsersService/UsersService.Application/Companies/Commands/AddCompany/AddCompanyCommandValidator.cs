using FluentValidation;
using Microsoft.AspNetCore.Http;
using UsersService.Domain.Constants;

namespace UsersService.Application.Companies.Commands.AddCompany
{
    public sealed class AddCompanyCommandValidator : AbstractValidator<AddCompanyCommand>
    {
        public AddCompanyCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("Name is required");

            RuleFor(c => c.Name)
                .MaximumLength(BusinessRules.Company.MaxNameLength)
                .WithMessage("Name is too long");

            RuleFor(c => c.Type)
                .NotNull()
                .NotEmpty()
                .WithMessage("Type is required");

            RuleFor(c => c.Type)
                .MaximumLength(BusinessRules.Company.MaxTypeLength)
                .WithMessage("Type is too long");

            RuleFor(c => c.City)
                .MaximumLength(BusinessRules.Company.MaxCityLength)
                .WithMessage("City is too long");

            RuleFor(c => c.Address)
                .MaximumLength(BusinessRules.Company.MaxAddressLength)
                .WithMessage("Address is too long");

            RuleFor(c => c.Email)
                .EmailAddress()
                .WithMessage("Incorrect email");

            RuleFor(c => c.Email)
                .MaximumLength(BusinessRules.Company.MaxEmailLength)
                .WithMessage("Email is too long");

            RuleFor(c => c.Unp)
                .NotNull()
                .NotEmpty()
                .WithMessage("UNP is required");

            RuleFor(c => c.Unp)
                .MaximumLength(BusinessRules.Company.MaxUnpLength)
                .WithMessage("UNP is too long");

            RuleFor(c => c.Unp)
                .Matches(@"^\d{9}$")
                .WithMessage("UNP must be 9 digits");

            RuleFor(c => c.Image)
                .Must(IsCorrectImageExtension);
        }

        private bool IsCorrectImageExtension(IFormFile image)
        {
            if(image is null)
            {
                return true;
            }

            return BusinessRules.Image.AllowedExtensions.Contains(Path.GetExtension(image.FileName));
        }
    }
}
