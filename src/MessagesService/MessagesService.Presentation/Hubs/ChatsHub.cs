using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagesService.Presentation.Hubs
{
    public class ChatsHub : Hub
    {
        private readonly ILogger<ChatsHub> _logger;

        public ChatsHub(ILogger<ChatsHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("[SignalR] [Connection] userId = {UserId}", Context.UserIdentifier);

            return base.OnConnectedAsync();
        }

        public async Task NotifyChatsUpdate(Guid recipientId)
        {
            await Clients.User($"{recipientId}").SendAsync("ChatsUpdate", Context.ConnectionAborted);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation(
                "[SignalR] [Connection] userId {UserId}, exception {@Exception}",
                Context.UserIdentifier,
                exception);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
