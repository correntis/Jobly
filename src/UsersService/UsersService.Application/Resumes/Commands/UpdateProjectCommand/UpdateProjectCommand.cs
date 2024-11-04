using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateProjectCommand
{
    public sealed record UpdateProjectCommand(
        string Id,
        IEnumerable<Project> Projects) : IRequest<string>;
}
