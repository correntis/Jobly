using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateJobExperience
{
    public sealed record UpdateJobExperienceCommand(
        string Id,
        IEnumerable<JobExperience> JobExperiences) : IRequest<string>;
}
