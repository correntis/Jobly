using MediatR;

namespace UsersService.Application.Companies.Commands.DeleteCompany
{
    public sealed record DeleteCompanyCommand(Guid Id) : IRequest<Guid>;
}
