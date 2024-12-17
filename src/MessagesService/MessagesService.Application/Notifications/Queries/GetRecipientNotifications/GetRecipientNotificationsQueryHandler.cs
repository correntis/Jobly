using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Notifications.Queries.GetRecipientNotifications
{
    public class GetRecipientNotificationsQueryHandler : IRequestHandler<GetRecipientNotificationsQuery, List<Notification>>
    {
        private readonly ILogger<GetRecipientNotificationsQueryHandler> _logger;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMapper _mapper;

        public GetRecipientNotificationsQueryHandler(
            ILogger<GetRecipientNotificationsQueryHandler> logger,
            INotificationsRepository notificationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _notificationsRepository = notificationsRepository;
            _mapper = mapper;
        }

        public async Task<List<Notification>> Handle(GetRecipientNotificationsQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for recipient {RecipientId}",
                request.GetType().Name,
                request.RecipientId);

            var notificationsEntities = await _notificationsRepository.GetPageBy(
                notif => notif.RecipientId,
                request.RecipientId,
                request.PageIndex,
                request.PageSize,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for recipient {RecipientId}",
                request.GetType().Name,
                request.RecipientId);

            return _mapper.Map<List<Notification>>(notificationsEntities);
        }
    }
}
