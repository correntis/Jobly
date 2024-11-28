using MediatR;

namespace UsersService.Application.Users.Commands.UpdateUserCommand
{
    public sealed record UpdateUserCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Phone) : IRequest<Guid>;
}
