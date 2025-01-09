using Microsoft.AspNetCore.SignalR;

namespace MessagesService.Presentation.Hubs.Providers
{
    internal class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var userId = connection.GetHttpContext()?.Request.Query["userId"];
            return userId ?? string.Empty;
        }
    }
}
