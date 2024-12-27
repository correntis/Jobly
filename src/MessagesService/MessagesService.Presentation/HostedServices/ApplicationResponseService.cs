using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Events;
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
    public class ApplicationResponseService : BackgroundService
    {
        private readonly ILogger<ApplicationResponseService> _logger;
        private readonly IBrokerConsumer<ApplicationResponseEvent> _consumer;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ITemplatesRepository _templatesRepository;
        private readonly ISender _sender;
        private readonly NotificationsService _notificationService;

        public ApplicationResponseService(
            ILogger<ApplicationResponseService> logger,
            IBrokerConsumer<ApplicationResponseEvent> consumer,
            INotificationsRepository notificationsRepository,
            ITemplatesRepository templatesRepository,
            ISender sender,
            NotificationsService notificationService)
        {
            _logger = logger;
            _consumer = consumer;
            _notificationsRepository = notificationsRepository;
            _templatesRepository = templatesRepository;
            _sender = sender;
            _notificationService = notificationService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.AddListener(Handler);

            return Task.CompletedTask;
        }

        private async Task Handler(object sender, BasicDeliverEventArgs args)
        {
            var responseEvent = JsonSerializer.Deserialize<ApplicationResponseEvent>(Encoding.UTF8.GetString(args.Body.ToArray()));

            _logger.LogInformation(
                "[Broker] Consume event {EventType} with message {@Message}",
                typeof(ApplicationResponseEvent),
                responseEvent);

            var notification = await _sender.Send(await GetSaveCommandAsync(responseEvent));

            await _notificationService.SendToUserAsync(notification.RecipientId, notification);

            _logger.LogInformation("[Broker] Message for user {UserId} successfully sent", notification.RecipientId);
        }

        private async Task<SaveNotificationCommand> GetSaveCommandAsync(ApplicationResponseEvent responseEvent)
        {
            var contentTemplate = await _templatesRepository.GetTemplateByEvent(nameof(ApplicationResponseEvent));

            var content = string.Format(
                contentTemplate,
                responseEvent.UserName,
                responseEvent.VacancyTitle,
                responseEvent.ApplicationStatus);

            return new SaveNotificationCommand(
                responseEvent.UserId,
                content,
                NotificationType.ApplicationResponse,
                new Dictionary<string, string>
                {
                    { nameof(ApplicationResponseEvent.UserId), responseEvent.UserId.ToString() },
                    { nameof(ApplicationResponseEvent.VacancyId), responseEvent.VacancyId.ToString() },
                    { nameof(ApplicationResponseEvent.ApplicationId), responseEvent.ApplicationId.ToString() },
                });
        }
    }
}
