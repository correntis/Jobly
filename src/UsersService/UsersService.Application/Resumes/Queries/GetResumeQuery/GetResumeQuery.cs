using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Queries.GetResumeQuery
{
    public sealed record GetResumeQuery(string Id) : IRequest<Resume>;
}
