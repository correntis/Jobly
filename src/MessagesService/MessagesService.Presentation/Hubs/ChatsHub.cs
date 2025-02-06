using MessagesService.Core.Constants;
using MessagesService.Presentation.Middleware.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessagesService.Presentation.Hubs
{
    [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
    [AuthorizeRole(Roles = BusinessRules.Roles.User)]
    public class ChatsHub : Hub
    {
        public async Task NotifyChatsUpdate(Guid recipientId)
        {
            await Clients.User($"{recipientId}").SendAsync("ChatsUpdate", Context.ConnectionAborted);
        }
    }
}
