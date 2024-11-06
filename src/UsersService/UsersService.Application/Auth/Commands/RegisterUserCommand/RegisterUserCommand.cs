using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Auth.Commands.RegisterUserCommand
{
    public sealed record RegisterUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Type) : IRequest<Token>;
}
