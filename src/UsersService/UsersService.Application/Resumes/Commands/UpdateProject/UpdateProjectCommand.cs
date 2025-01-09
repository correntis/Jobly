using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateProject
{
    public sealed record UpdateProjectCommand(
        string Id,
        IEnumerable<Project> Projects) : IRequest<string>;
}
