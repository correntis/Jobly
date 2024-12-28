using MediatR;

namespace UsersService.Application.Users.Commands.DeleteUser
{
    public sealed record DeleteUserCommand(Guid Id) : IRequest<Guid>;
}
