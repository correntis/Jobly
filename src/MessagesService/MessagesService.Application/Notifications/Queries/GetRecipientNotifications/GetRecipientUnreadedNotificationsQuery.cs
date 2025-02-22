using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Notifications.Queries.GetRecipientNotifications
{
    public sealed record GetRecipientUnreadedNotificationsQuery(Guid RecipientId) : IRequest<List<Notification>>;
}
