using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateLanguage
{
    public sealed record UpdateLanguageCommand(
        string Id,
        IEnumerable<Language> Languages) : IRequest<string>;
}
