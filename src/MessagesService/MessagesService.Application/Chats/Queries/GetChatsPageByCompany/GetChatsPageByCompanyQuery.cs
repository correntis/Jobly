using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Chats.Queries.GetChatsPageByCompany
{
    public sealed record GetChatsPageByCompanyQuery(
        Guid CompanyId,
        int PageIndex,
        int PageSize) : IRequest<List<Chat>>;
}
