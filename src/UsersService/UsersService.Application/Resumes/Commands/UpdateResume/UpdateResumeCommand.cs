using MediatR;

namespace UsersService.Application.Resumes.Commands.UpdateResume
{
    public sealed record UpdateResumeCommand(
        string Id,
        Guid UserId,
        string Title,
        string Summary,
        List<string> Skills,
        List<string> Tags) : IRequest<string>;
}
