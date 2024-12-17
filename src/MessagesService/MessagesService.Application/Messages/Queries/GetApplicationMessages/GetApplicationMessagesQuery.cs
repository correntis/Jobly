using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Messages.Queries.GetApplicationMessages
{
    public sealed record GetApplicationMessagesQuery(
        Guid ApplicationId,
        int PageIndex,
        int PageSize) : IRequest<List<Message>>;
}
