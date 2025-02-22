using AutoMapper;
using MediatR;
using MessagesService.Core.Enums;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Notifications.Queries.GetRecipientNotifications
{
    public class GetRecipientUnreadedNotificationsQueryHandler : IRequestHandler<GetRecipientUnreadedNotificationsQuery, List<Notification>>
    {
        private readonly ILogger<GetRecipientUnreadedNotificationsQueryHandler> _logger;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMapper _mapper;

        public GetRecipientUnreadedNotificationsQueryHandler(
            ILogger<GetRecipientUnreadedNotificationsQueryHandler> logger,
            INotificationsRepository notificationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _notificationsRepository = notificationsRepository;
            _mapper = mapper;
        }

        public async Task<List<Notification>> Handle(GetRecipientUnreadedNotificationsQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for recipient {RecipientId}",
                request.GetType().Name,
                request.RecipientId);

            var notificationsEntities = await _notificationsRepository.GetAllWithStatusBy(
                notif => notif.RecipientId,
                request.RecipientId,
                (int)NotificationStatus.Sent,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for recipient {RecipientId}",
                request.GetType().Name,
                request.RecipientId);

            return _mapper.Map<List<Notification>>(notificationsEntities);
        }
    }
}
