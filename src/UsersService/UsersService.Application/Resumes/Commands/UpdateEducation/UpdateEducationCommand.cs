using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateEducation
{
    public sealed record UpdateEducationCommand(
        string Id,
        IEnumerable<Education> Educations) : IRequest<string>;
}
