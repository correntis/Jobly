using MediatR;

namespace UsersService.Application.Companies.Queries.IsCompanyExists
{
    public sealed record IsCompanyExistsQuery(Guid Id) : IRequest<bool>;
}
