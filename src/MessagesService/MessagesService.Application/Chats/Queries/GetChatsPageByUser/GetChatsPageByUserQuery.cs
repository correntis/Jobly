using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Chats.Queries.GetPageByUser
{
    public sealed record GetChatsPageByUserQuery(
        Guid UserId,
        int PageIndex,
        int PageSize) : IRequest<List<Chat>>;
}
