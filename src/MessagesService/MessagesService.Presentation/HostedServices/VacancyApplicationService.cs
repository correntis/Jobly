using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Events;
using MediatR;
using MessagesService.Application.Chats.Commands.AddChat;
using MessagesService.Presentation.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MessagesService.Presentation.HostedServices
{
    public class VacancyApplicationService : BackgroundService
    {
        private readonly ILogger<VacancyApplicationService> _logger;
        private readonly IBrokerConsumer<VacancyApplicationEvent> _consumer;
        private readonly ISender _sender;
        private readonly ChatsService _chatsService;

        public VacancyApplicationService(
            ILogger<VacancyApplicationService> logger,
            IBrokerConsumer<VacancyApplicationEvent> consumer,
            ISender sender,
            ChatsService chatsService)
        {
            _logger = logger;
            _consumer = consumer;
            _sender = sender;
            _chatsService = chatsService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.AddListener(Handler);

            return Task.CompletedTask;
        }

        private async Task Handler(object sender, BasicDeliverEventArgs args)
        {
            var applicationEvent = JsonSerializer.Deserialize<VacancyApplicationEvent>(Encoding.UTF8.GetString(args.Body.ToArray()));

            _logger.LogInformation(
                "[Broker] Consume event {EventType} with message {@Message}",
                typeof(VacancyApplicationEvent),
                applicationEvent);

            var chat = await _sender.Send(GetAddCommand(applicationEvent));

            await _chatsService.NotifyAboutNewChatAsync(chat);

            _logger.LogInformation("[Broker] Message for user {UserId} successfully sent", chat.UserId);
        }

        private AddChatCommand GetAddCommand(VacancyApplicationEvent applicationEvent)
        {
            return new AddChatCommand(
                applicationEvent.UserId,
                applicationEvent.CompanyId,
                applicationEvent.ApplicationId,
                applicationEvent.VacacnyId);
        }
    }
}
