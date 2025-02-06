using MediatR;

namespace MessagesService.Application.Notifications.Commands.ViewNotification
{
    public sealed record ViewNotificationCommand(List<string> NotificationsIds) : IRequest;
}
