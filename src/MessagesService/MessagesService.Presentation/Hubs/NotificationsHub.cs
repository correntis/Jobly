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

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("[SignalR] [Connection] Notifications userId = {userId}", Context.UserIdentifier);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("[SignalR] [Disconnect] Notifications exception {@Exception}", exception);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
