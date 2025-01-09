using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Messages.Queries.SearchChatMessages
{
    public sealed record SearchChatMessagesQuery(
        string ChatId,
        string Content) : IRequest<List<Message>>;
}
