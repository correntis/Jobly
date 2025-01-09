using MediatR;
using MessagesService.Core.Enums;
using MessagesService.Core.Models;

namespace MessagesService.Application.Notifications.Commands.SaveNotification
{
    public sealed record SaveNotificationCommand(
        Guid RecipientId,
        string Content,
        NotificationType Type,
        Dictionary<string, string> Metadata) : IRequest<Notification>;
}
