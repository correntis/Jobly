using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Models;
using MediatR;
using MessagesService.Application.Notifications.Commands.SaveNotification;
using MessagesService.Core.Enums;
using MessagesService.DataAccess.Abstractions;
using MessagesService.Presentation.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MessagesService.Presentation.HostedServices
{
    public class UserRegistrationService : BackgroundService
    {
        private readonly ILogger<UserRegistrationService> _logger;
        private readonly IBrokerConsumer<RegistrationEvent> _consumer;
        private readonly NotificationsService _notificationService;
        private readonly ISender _sender;
        private readonly ITemplatesRepository _templatesRepository;

        public UserRegistrationService(
            ILogger<UserRegistrationService> logger,
            IBrokerConsumer<RegistrationEvent> consumer,
            ISender sender,
            ITemplatesRepository templatesRepository,
            NotificationsService notificationService)
        {
            _logger = logger;
            _consumer = consumer;
            _notificationService = notificationService;
            _sender = sender;
            _templatesRepository = templatesRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.AddListener(Handler);

            return Task.CompletedTask;
        }

        private async Task Handler(object sender, BasicDeliverEventArgs args)
        {
            var registrationEvent = JsonSerializer.Deserialize<RegistrationEvent>(Encoding.UTF8.GetString(args.Body.ToArray()));

            _logger.LogInformation(
                "[Broker] Consume event {EventType} with message {@Message}",
                typeof(RegistrationEvent),
                registrationEvent);

            var notification = await _sender.Send(await GetSaveCommandAsync(registrationEvent));

            await _notificationService.SendToUserAsync(notification.RecipientId, notification);

            _logger.LogInformation("[Broker] Message for user {UserId} successfully sent", notification.RecipientId);
        }

        private async Task<SaveNotificationCommand> GetSaveCommandAsync(RegistrationEvent registrationEvent)
        {
            var contentTemplate = await _templatesRepository.GetTemplateByEvent(nameof(RegistrationEvent));

            var content = string.Format(contentTemplate, registrationEvent.UserName);

            return new SaveNotificationCommand(
                registrationEvent.UserId,
                content,
                NotificationType.Registration,
                new Dictionary<string, string>
                {
                    { nameof(RegistrationEvent.UserId), registrationEvent.UserId.ToString() },
                });
        }
    }
}
