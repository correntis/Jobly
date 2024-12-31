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
    public class RecomendVacancyService : BackgroundService
    {
        private readonly ILogger<RecomendVacancyService> _logger;
        private readonly IBrokerConsumer<RecomendVacancyEvent> _consumer;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ITemplatesRepository _templatesRepository;
        private readonly ISender _sender;
        private readonly NotificationsService _notificationService;

        public RecomendVacancyService(
            ILogger<RecomendVacancyService> logger,
            IBrokerConsumer<RecomendVacancyEvent> consumer,
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
            var recomendEvent = JsonSerializer.Deserialize<RecomendVacancyEvent>(Encoding.UTF8.GetString(args.Body.ToArray()));

            _logger.LogInformation(
                "[Broker] Consume event {EventType} with message {@Message}",
                typeof(RecomendVacancyEvent),
                recomendEvent);

            var notification = await _sender.Send(await GetSaveCommandAsync(recomendEvent));

            await _notificationService.SendToUserAsync(notification.RecipientId, notification);

            _logger.LogInformation("[Broker] Message for user {UserId} successfully sent", notification.RecipientId);
        }

        private async Task<SaveNotificationCommand> GetSaveCommandAsync(RecomendVacancyEvent recomendEvent)
        {
            var contentTemplate = await _templatesRepository.GetTemplateByEvent(nameof(RecomendVacancyEvent));

            var content = string.Format(
                contentTemplate,
                Math.Round(recomendEvent.MatchScore * 100),
                recomendEvent.VacancyName);

            return new SaveNotificationCommand(
                recomendEvent.UserId,
                content,
                NotificationType.ApplicationResponse,
                new Dictionary<string, string>
                {
                    { nameof(RecomendVacancyEvent.UserId), recomendEvent.UserId.ToString() },
                    { nameof(RecomendVacancyEvent.VacancyId), recomendEvent.VacancyId.ToString() },
                });
        }
    }
}
