using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace MessagesService.Presentation.Services
{
    public class TelegramBotService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly IConfiguration _configuration;
        private ITelegramBotClient? _botClient;

        public TelegramBotService(
            ILogger<TelegramBotService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public ITelegramBotClient GetBotClient()
        {
            if (_botClient == null)
            {
                var botToken = _configuration["Telegram:BotToken"];
                if (string.IsNullOrEmpty(botToken))
                {
                    throw new InvalidOperationException("Telegram bot token is not configured");
                }

                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                _botClient = new TelegramBotClient(botToken, httpClient);
            }

            return _botClient;
        }

        public async Task SendMessageAsync(long chatId, string message)
        {
            try
            {
                var botClient = GetBotClient();
                await botClient.SendTextMessageAsync(chatId, message);
                _logger.LogInformation("[Telegram] Message sent to chat {ChatId}", chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Telegram] Failed to send message to chat {ChatId}", chatId);
                throw;
            }
        }
    }
}

