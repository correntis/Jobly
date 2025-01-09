using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Chats.Commands.AddChat
{
    public sealed record AddChatCommand(
        Guid UserId,
        Guid CompanyId,
        Guid ApplicationId,
        Guid VacancyId) : IRequest<Chat>;
}
