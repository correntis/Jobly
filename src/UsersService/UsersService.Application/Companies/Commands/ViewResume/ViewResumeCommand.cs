using MediatR;

namespace UsersService.Application.Companies.Commands.ViewResume
{
    public sealed record ViewResumeCommand(
        Guid CompanyId,
        string ResumeId) : IRequest;
}
