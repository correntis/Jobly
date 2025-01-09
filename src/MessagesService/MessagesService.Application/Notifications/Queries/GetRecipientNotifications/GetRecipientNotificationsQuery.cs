using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Notifications.Queries.GetRecipientNotifications
{
    public sealed record GetRecipientNotificationsQuery(
        Guid RecipientId,
        int PageIndex,
        int PageSize) : IRequest<List<Notification>>;
}
