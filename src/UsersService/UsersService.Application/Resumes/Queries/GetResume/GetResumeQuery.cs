using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Queries.GetResume
{
    public sealed record GetResumeQuery(string Id) : IRequest<Resume>;
}
