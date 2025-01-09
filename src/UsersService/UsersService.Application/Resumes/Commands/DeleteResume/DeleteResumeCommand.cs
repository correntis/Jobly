using MediatR;

namespace UsersService.Application.Resumes.Commands.DeleteResume
{
    public sealed record DeleteResumeCommand(string Id) : IRequest<string>;
}
