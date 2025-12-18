using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using MessagesService.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessagesService.Presentation.Services
{
    public class NotificationsService
    {
        private readonly ILogger<NotificationsService> _logger;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly TelegramBotService _telegramBotService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NotificationsService(
            ILogger<NotificationsService> logger,
            IHubContext<NotificationsHub> hubContext,
            TelegramBotService telegramBotService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _hubContext = hubContext;
            _telegramBotService = telegramBotService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task SendToUserAsync(Guid recipientId, Notification notification)
        {
            _logger.LogInformation("[SingalR] Send notification {@Notification} to recipient {RecipientId}", notification, recipientId);

            // Отправка через SignalR
            await _hubContext.Clients
                .User(recipientId.ToString())
                .SendAsync("ReceiveNotification", notification);

            // Отправка в Telegram, если пользователь подключен
            await SendToTelegramAsync(recipientId, notification);
        }

        public async Task SendToUsersAsync(IEnumerable<Guid> recipientIds, Notification notification)
        {
            _logger.LogInformation("[SingalR] Send notification {@Notification} to recipients {RecipientsIds}", notification, recipientIds);

            // Отправка через SignalR
            await _hubContext.Clients.Users(recipientIds.Select(id => id.ToString()))
                .SendAsync("ReceiveNotification", notification);

            // Отправка в Telegram для каждого пользователя
            foreach (var recipientId in recipientIds)
            {
                await SendToTelegramAsync(recipientId, notification);
            }
        }

        private async Task SendToTelegramAsync(Guid recipientId, Notification notification)
        {
            try
            {
                // Создаем scope для использования scoped сервиса
                using var scope = _serviceScopeFactory.CreateScope();
                var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

                var telegramChatId = await usersService.GetUserTelegramChatIdAsync(recipientId);
                
                if (telegramChatId.HasValue)
                {
                    await _telegramBotService.SendMessageAsync(telegramChatId.Value, notification.Content);
                    _logger.LogInformation("[Telegram] Notification sent to user {UserId} in chat {ChatId}", recipientId, telegramChatId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Telegram] Failed to send notification to user {UserId}", recipientId);
                // Не прерываем выполнение, если не удалось отправить в Telegram
            }
        }
    }
}
