using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Messages.Queries.GetChatMessages
{
    public sealed record GetChatMessagesQuery(
        string ChatId,
        int PageIndex,
        int PageSize) : IRequest<List<Message>>;
}
