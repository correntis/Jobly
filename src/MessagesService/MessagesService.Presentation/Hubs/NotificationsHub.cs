using MessagesService.Core.Constants;
using MessagesService.Presentation.Middleware.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessagesService.Presentation.Hubs
{
    [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
    [AuthorizeRole(Roles = BusinessRules.Roles.User)]
    public class NotificationsHub : Hub
    {
    }
}
