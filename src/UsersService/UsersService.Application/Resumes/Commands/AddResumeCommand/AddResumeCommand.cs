using MediatR;

namespace UsersService.Application.Resumes.Commands.AddResumeCommand
{
    public sealed record AddResumeCommand(
        Guid UserId,
        string Title,
        string Summary,
        List<string> Skills,
        List<string> Tags) : IRequest<string>;
}
