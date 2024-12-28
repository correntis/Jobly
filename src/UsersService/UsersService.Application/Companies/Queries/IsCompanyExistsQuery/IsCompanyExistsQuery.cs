using MediatR;

namespace UsersService.Application.Companies.Queries.IsCompanyExistsQuery
{
    public sealed record IsCompanyExistsQuery(Guid Id) : IRequest<bool>;
}
