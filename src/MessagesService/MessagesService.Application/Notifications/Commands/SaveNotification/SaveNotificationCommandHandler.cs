using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Notifications.Commands.SaveNotification
{
    public class SaveNotificationCommandHandler : IRequestHandler<SaveNotificationCommand, Notification>
    {
        private readonly ILogger<SaveNotificationCommandHandler> _logger;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMapper _mapper;

        public SaveNotificationCommandHandler(
            ILogger<SaveNotificationCommandHandler> logger,
            INotificationsRepository notificationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _notificationsRepository = notificationsRepository;
            _mapper = mapper;
        }

        public async Task<Notification> Handle(SaveNotificationCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for recipient {RecipientId}",
                request.GetType().Name,
                request.RecipientId);

            var notificationEntity = _mapper.Map<NotificationEntity>(request);

            await _notificationsRepository.AddAsync(notificationEntity, token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for recipient {RecipientId}",
                request.GetType().Name,
                request.RecipientId);

            return _mapper.Map<Notification>(notificationEntity);
        }
    }
}
