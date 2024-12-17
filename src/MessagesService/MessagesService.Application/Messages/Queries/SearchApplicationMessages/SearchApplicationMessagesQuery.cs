using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Messages.Queries.SearchApplicationMessages
{
    public sealed record SearchApplicationMessagesQuery(
        Guid ApplicationId,
        string Content) : IRequest<List<Message>>;
}

