using MediatR;

namespace UsersService.Application.Companies.Commands.DeleteCompanyCommand
{
    public sealed record DeleteCompanyCommand(int Id) : IRequest<int>;
}
