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
    public class ResumeViewService : BackgroundService
    {
        private readonly ILogger<ResumeViewService> _logger;
        private readonly IBrokerConsumer<ResumeViewEvent> _consumer;
        private readonly NotificationsService _notificationService;
        private readonly ISender _sender;
        private readonly ITemplatesRepository _templatesRepository;

        public ResumeViewService(
            ILogger<ResumeViewService> logger,
            IBrokerConsumer<ResumeViewEvent> consumer,
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
            var resumeViewEvent = JsonSerializer.Deserialize<ResumeViewEvent>(Encoding.UTF8.GetString(args.Body.ToArray()));

            _logger.LogInformation(
                "[Broker] Consume event {EventType} with message {@Message}",
                typeof(ResumeViewEvent),
                resumeViewEvent);

            var notification = await _sender.Send(await GetSaveCommandAsync(resumeViewEvent));

            await _notificationService.SendToUserAsync(notification.RecipientId, notification);

            _logger.LogInformation("[Broker] Message for user {UserId} successfully sent", notification.RecipientId);
        }

        private async Task<SaveNotificationCommand> GetSaveCommandAsync(ResumeViewEvent resumeViewEvent)
        {
            var contentTemplate = await _templatesRepository.GetTemplateByEvent(nameof(ResumeViewEvent));

            var content = string.Format(contentTemplate, resumeViewEvent.CompanyName);

            return new SaveNotificationCommand(
                resumeViewEvent.UserId,
                content,
                NotificationType.ResumeView,
                new Dictionary<string, string>
                {
                    { nameof(ResumeViewEvent.UserId), resumeViewEvent.UserId.ToString() },
                    { nameof(ResumeViewEvent.CompanyId), resumeViewEvent.CompanyId.ToString() },
                });
        }
    }
}
