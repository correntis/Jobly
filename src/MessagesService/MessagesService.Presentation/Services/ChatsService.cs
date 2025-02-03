using MessagesService.Core.Models;
using MessagesService.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagesService.Presentation.Services
{
    public class ChatsService
    {
        private readonly ILogger<ChatsService> _logger;
        private readonly IHubContext<ChatsHub> _hubContext;

        public ChatsService(
            ILogger<ChatsService> logger,
            IHubContext<ChatsHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task NotifyAboutNewChatAsync(Chat newChat)
        {
            await _hubContext.Clients
                .Users($"{newChat.UserId}", $"{newChat.CompanyId}")
                .SendAsync("NewChat", newChat);
        }
    }
}
