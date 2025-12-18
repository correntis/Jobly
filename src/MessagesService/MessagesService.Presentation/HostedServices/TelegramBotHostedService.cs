using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MessagesService.DataAccess.Abstractions;
using MessagesService.Presentation.Services;

namespace MessagesService.Presentation.HostedServices
{
    public class TelegramBotHostedService : BackgroundService
    {
        private readonly ILogger<TelegramBotHostedService> _logger;
        private readonly TelegramBotService _telegramBotService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private ITelegramBotClient? _botClient;

        public TelegramBotHostedService(
            ILogger<TelegramBotHostedService> logger,
            TelegramBotService telegramBotService,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _telegramBotService = telegramBotService;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _botClient = _telegramBotService.GetBotClient();

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                };

                _botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken
                );

                var me = await _botClient.GetMeAsync(stoppingToken);
                _logger.LogInformation("[Telegram] Bot @{BotUsername} started", me.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Telegram] Failed to start bot");
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            _logger.LogInformation(
                "[Telegram] Received message '{MessageText}' in chat {ChatId} from user {UserId}",
                messageText,
                chatId,
                message.From?.Id);

            if (messageText.StartsWith("/start"))
            {
                await HandleStartCommandAsync(message, cancellationToken);
            }
        }

        private async Task HandleStartCommandAsync(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var messageText = message.Text ?? string.Empty;

            try
            {
                // Извлекаем userId из команды /start {userId}
                var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !Guid.TryParse(parts[1], out var userId))
                {
                    await _botClient!.SendTextMessageAsync(
                        chatId,
                        "Неверная команда. Пожалуйста, используйте ссылку из личного кабинета для подключения.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // Создаем scope для использования scoped сервиса
                using var scope = _serviceScopeFactory.CreateScope();
                var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

                // Сохраняем chatId для пользователя через gRPC
                var success = await usersService.UpdateTelegramChatIdAsync(userId, chatId, cancellationToken);
                
                if (!success)
                {
                    await _botClient!.SendTextMessageAsync(
                        chatId,
                        "❌ Произошла ошибка при сохранении данных. Пожалуйста, попробуйте позже.",
                        cancellationToken: cancellationToken);
                    return;
                }

                await _botClient!.SendTextMessageAsync(
                    chatId,
                    "✅ Телеграм бот успешно подключен! Теперь вы будете получать уведомления здесь.",
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "[Telegram] Successfully connected user {UserId} with chat {ChatId}",
                    userId,
                    chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Telegram] Failed to handle start command for chat {ChatId}", chatId);

                await _botClient!.SendTextMessageAsync(
                    chatId,
                    "❌ Произошла ошибка при подключении. Пожалуйста, попробуйте позже.",
                    cancellationToken: cancellationToken);
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "[Telegram] Polling error occurred");
            return Task.CompletedTask;
        }
    }
}

