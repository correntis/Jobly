using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Queries.GetResumeByUser
{
    public sealed record GetResumeByUserQuery(int UserId) : IRequest<Resume>;
}
