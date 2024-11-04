using MediatR;

namespace UsersService.Application.Companies.Commands.AddCompanyCommand
{
    public sealed record AddCompanyCommand(
        int UserId,
        string Name,
        string City,
        string Address,
        string Email,
        string Type) : IRequest<int>;
}
