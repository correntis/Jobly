using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Auth.Commands.LoginUserCommand
{
    public sealed record LoginUserCommand(
        string Email,
        string Password) : IRequest<(User, Token)>;
}
