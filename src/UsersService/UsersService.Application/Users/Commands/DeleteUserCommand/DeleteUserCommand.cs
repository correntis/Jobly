using MediatR;

namespace UsersService.Application.Users.Commands.DeleteUserCommand
{
    public sealed record DeleteUserCommand(Guid Id) : IRequest<Guid>;
}
