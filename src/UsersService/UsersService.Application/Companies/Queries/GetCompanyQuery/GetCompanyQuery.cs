using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Queries.GetCompanyQuery
{
    public sealed record GetCompanyQuery(Guid Id) : IRequest<Company>;
}
