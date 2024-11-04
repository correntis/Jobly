using MediatR;

namespace UsersService.Application.Users.Commands.DeleteUserCommand
{
    public sealed record DeleteUserCommand(int Id) : IRequest<int>;
}
