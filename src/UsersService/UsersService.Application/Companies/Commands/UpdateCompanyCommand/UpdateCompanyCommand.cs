using MediatR;
using Microsoft.AspNetCore.Http;

namespace UsersService.Application.Companies.Commands.UpdateCompanyCommand
{
    public sealed record UpdateCompanyCommand(
        int Id,
        string Name,
        string Description,
        string City,
        string Address,
        string Email,
        string Phone,
        string WebSite,
        string Type,
        IFormFile Image
        ) : IRequest<int>;
}
