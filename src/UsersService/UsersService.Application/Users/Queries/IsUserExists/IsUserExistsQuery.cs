using MediatR;

namespace UsersService.Application.Users.Queries.IsUserExists
{
    public sealed record IsUserExistsQuery(Guid Id) : IRequest<bool>;
}
