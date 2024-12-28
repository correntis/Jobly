using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Users.Queries.GetUser
{
    public sealed record GetUserQuery(Guid Id) : IRequest<User>;
}
