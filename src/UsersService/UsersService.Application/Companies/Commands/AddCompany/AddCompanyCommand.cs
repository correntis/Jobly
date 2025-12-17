using MediatR;
using Microsoft.AspNetCore.Http;

namespace UsersService.Application.Companies.Commands.AddCompany
{
    public sealed record AddCompanyCommand(
        Guid UserId,
        string Name,
        string City,
        string Address,
        string Email,
        string Phone,
        string WebSite,
        string Type,
        string Description,
        string Unp,
        IFormFile Image) : IRequest<Guid>;
}
