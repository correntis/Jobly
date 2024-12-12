using MediatR;

namespace UsersService.Application.Resumes.Commands.DeleteResumeCommand
{
    public sealed record DeleteResumeCommand(string Id) : IRequest<string>;
}
