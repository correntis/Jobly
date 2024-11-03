using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Users.Queries.GetUserQuery
{
    public sealed record GetUserQuery(int Id) : IRequest<User>;
}
