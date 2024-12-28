using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Queries.GetCompany
{
    public sealed record GetCompanyQuery(Guid Id) : IRequest<Company>;
}
