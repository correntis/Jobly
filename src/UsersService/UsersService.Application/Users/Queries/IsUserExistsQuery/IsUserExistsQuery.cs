using MediatR;

namespace UsersService.Application.Users.Queries.IsUserExistsQuery
{
    public sealed record IsUserExistsQuery(Guid Id) : IRequest<bool>;
}
