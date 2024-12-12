using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateJobExperienceCommand
{
    public sealed record UpdateJobExperienceCommand(
        string Id,
        IEnumerable<JobExpirience> JobExpiriences) : IRequest<string>;
}
