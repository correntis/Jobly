using MediatR;

namespace UsersService.Application.Companies.Commands.DeleteCompanyCommand
{
    public sealed record DeleteCompanyCommand(Guid Id) : IRequest<Guid>;
}
