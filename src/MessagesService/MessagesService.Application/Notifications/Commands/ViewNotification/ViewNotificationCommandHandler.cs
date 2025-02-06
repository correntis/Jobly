using MediatR;
using MessagesService.Core.Enums;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Notifications.Commands.ViewNotification
{
    public class ViewNotificationCommandHandler : IRequestHandler<ViewNotificationCommand>
    {
        private readonly ILogger<ViewNotificationCommandHandler> _logger;
        private readonly INotificationsRepository _notificationsRepository;

        public ViewNotificationCommandHandler(
            ILogger<ViewNotificationCommandHandler> logger,
            INotificationsRepository notificationsRepository)
        {
            _logger = logger;
            _notificationsRepository = notificationsRepository;
        }

        public async Task Handle(ViewNotificationCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for notifications {@NotificationsIds}",
                request.GetType().Name,
                request.NotificationsIds);

            await _notificationsRepository.SetManyByIdsAsync(
                request.NotificationsIds,
                notif => notif.Status,
                (int)NotificationStatus.Viewed,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for notification {@NotificationsIds}",
                request.GetType().Name,
                request.NotificationsIds);
        }
    }
}
