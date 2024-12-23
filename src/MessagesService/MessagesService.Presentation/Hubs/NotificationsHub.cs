using MessagesService.Core.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagesService.Presentation.Hubs
{
    public class NotificationsHub : Hub
    {
        private readonly ILogger<NotificationsHub> _logger;

        public NotificationsHub(ILogger<NotificationsHub> logger)
        {
            _logger = logger;
        }

        public async Task SendNotification(Guid recipientId, Notification notification)
        {
            await Clients.User($"{recipientId}").SendAsync("ReceiveNotification", notification, Context.ConnectionAborted);
        }

        public async Task SendBroadcastNotification(List<string> recipientsIds, Notification notification)
        {
            await Clients.Users(recipientsIds).SendAsync("ReceiveNotification", notification, Context.ConnectionAborted);
        }
    }
}
