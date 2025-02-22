using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Chats.Queries.GetChatByApplication
{
    public record GetChatByApplicationQuery(Guid ApplicationId) : IRequest<Chat>;
}
