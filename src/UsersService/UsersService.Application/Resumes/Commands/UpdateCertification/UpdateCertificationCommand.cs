using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateCertification
{
    public sealed record UpdateCertificationCommand(
        string Id,
        List<Certification> Certifications) : IRequest<string>;
}
