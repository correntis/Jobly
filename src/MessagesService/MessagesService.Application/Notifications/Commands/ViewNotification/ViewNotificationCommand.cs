using MediatR;

namespace MessagesService.Application.Notifications.Commands.ViewNotification
{
    public sealed record ViewNotificationCommand(string NotificaitonId) : IRequest;
}
