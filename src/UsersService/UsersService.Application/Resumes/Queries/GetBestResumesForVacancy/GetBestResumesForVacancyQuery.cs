using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Queries.GetBestResumesForVacancy
{
    public sealed record GetBestResumesForVacancyQuery(
        List<string> Skills,
        List<string> Tags,
        List<Language> Languages) : IRequest<List<Resume>>;
}
