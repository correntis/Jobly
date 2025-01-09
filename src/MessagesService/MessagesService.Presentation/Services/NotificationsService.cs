using MessagesService.Core.Models;
using MessagesService.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagesService.Presentation.Services
{
    public class NotificationsService
    {
        private readonly ILogger<NotificationsService> _logger;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public NotificationsService(
            ILogger<NotificationsService> logger,
            IHubContext<NotificationsHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(Guid recipientId, Notification notification)
        {
            _logger.LogInformation("[SingalR] Send notification {@Notification} to recipient {RecipientId}", notification, recipientId);

            await _hubContext.Clients
                .User(recipientId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task SendToUsersAsync(IEnumerable<Guid> recipientIds, Notification notification)
        {
            _logger.LogInformation("[SingalR] Send notification {@Notification} to recipients {RecipientsIds}", notification, recipientIds);

            await _hubContext.Clients.Users(recipientIds.Select(id => id.ToString()))
                .SendAsync("ReceiveNotification", notification);
        }
    }
}
