using MediatR;

namespace UsersService.Application.Resumes.Commands.UpdateResumeCommand
{
    public sealed record UpdateResumeCommand(
        string Id,
        Guid UserId,
        string Title,
        string Summary,
        List<string> Skills,
        List<string> Tags) : IRequest<string>;
}
