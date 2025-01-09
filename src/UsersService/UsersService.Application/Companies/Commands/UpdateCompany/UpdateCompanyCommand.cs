using MediatR;
using Microsoft.AspNetCore.Http;

namespace UsersService.Application.Companies.Commands.UpdateCompany
{
    public sealed record UpdateCompanyCommand(
        Guid Id,
        string Name,
        string Description,
        string City,
        string Address,
        string Email,
        string Phone,
        string WebSite,
        string Type,
        IFormFile Image
        ) : IRequest<Guid>;
}
