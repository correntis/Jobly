using MediatR;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateLanguageCommand
{
    public sealed record UpdateLanguageCommand(
        string Id,
        IEnumerable<Language> Languages) : IRequest<string>;
}
