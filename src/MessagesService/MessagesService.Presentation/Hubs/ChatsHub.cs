using Microsoft.AspNetCore.SignalR;

namespace MessagesService.Presentation.Hubs
{
    public class ChatsHub : Hub
    {
        public async Task NotifyChatsUpdate(Guid recipientId)
        {
            await Clients.User($"{recipientId}").SendAsync("ChatsUpdate", Context.ConnectionAborted);
        }
    }
}
