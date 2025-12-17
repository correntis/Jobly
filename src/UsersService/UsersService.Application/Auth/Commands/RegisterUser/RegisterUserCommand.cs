using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Auth.Commands.RegisterUser
{
    public sealed record RegisterUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        List<string> RolesNames,
        bool IsFullRegistration = true) : IRequest<(Guid, Token)>;
}
