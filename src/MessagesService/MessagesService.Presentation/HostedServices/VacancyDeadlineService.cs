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
    public class VacancyDeadlineService : BackgroundService
    {
        private readonly ILogger<VacancyDeadlineService> _logger;
        private readonly IBrokerConsumer<LikedVacancyDeadlineEvent> _consumer;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ITemplatesRepository _templatesRepository;
        private readonly ISender _sender;
        private readonly NotificationsService _notificationService;

        public VacancyDeadlineService(
            ILogger<VacancyDeadlineService> logger,
            IBrokerConsumer<LikedVacancyDeadlineEvent> consumer,
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
            var deadlineEvent = JsonSerializer.Deserialize<LikedVacancyDeadlineEvent>(Encoding.UTF8.GetString(args.Body.ToArray()));

            _logger.LogInformation(
                "[Broker] Consume event {EventType} with message {@Message}",
                typeof(LikedVacancyDeadlineEvent),
                deadlineEvent);

            var notification = await _sender.Send(await GetSaveCommandAsync(deadlineEvent));

            await _notificationService.SendToUserAsync(notification.RecipientId, notification);

            _logger.LogInformation("[Broker] Message for user {UserId} successfully sent", notification.RecipientId);
        }

        private async Task<SaveNotificationCommand> GetSaveCommandAsync(LikedVacancyDeadlineEvent deadlineEvent)
        {
            var contentTemplate = await _templatesRepository.GetTemplateByEvent(nameof(LikedVacancyDeadlineEvent));

            var content = string.Format(
                contentTemplate,
                deadlineEvent.VacancyName,
                (deadlineEvent.VacancyDeadlineAt - DateTime.UtcNow).Days);

            return new SaveNotificationCommand(
                deadlineEvent.UserId,
                content,
                NotificationType.LikedVacancyDeadline,
                new Dictionary<string, string>
                {
                    { nameof(LikedVacancyDeadlineEvent.UserId), deadlineEvent.UserId.ToString() },
                    { nameof(LikedVacancyDeadlineEvent.VacancyId), deadlineEvent.VacancyId.ToString() },
                });
        }
    }
}
