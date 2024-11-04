using MediatR;

namespace UsersService.Application.Users.Commands.AddUserCommand
{
    public sealed record AddUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Type) : IRequest<int>;
}
