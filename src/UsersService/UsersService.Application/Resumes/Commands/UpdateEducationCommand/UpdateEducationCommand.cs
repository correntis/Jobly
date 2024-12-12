using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateEducationCommand
{
    public sealed record UpdateEducationCommand(
        string Id,
        IEnumerable<Education> Educations) : IRequest<string>;
}
