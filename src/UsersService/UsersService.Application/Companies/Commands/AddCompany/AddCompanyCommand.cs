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
        string Type,
        IFormFile Image) : IRequest<Guid>;
}
