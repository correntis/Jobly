using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Queries.GetCompanyByUser
{
    public sealed record GetCompanyByUserQuery(Guid UserId) : IRequest<Company>;

}
