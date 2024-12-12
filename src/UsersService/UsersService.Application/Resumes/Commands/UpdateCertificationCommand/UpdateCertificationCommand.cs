using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateCertificationCommand
{
    public sealed record UpdateCertificationCommand(
        string Id,
        List<Certification> Certifications) : IRequest<string>;
}
