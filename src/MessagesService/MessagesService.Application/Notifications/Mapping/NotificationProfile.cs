using AutoMapper;
using MessagesService.Application.Notifications.Commands.SaveNotification;
using MessagesService.Core.Enums;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Entities;

namespace MessagesService.Application.Notifications.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationEntity, Notification>();

            CreateMap<SaveNotificationCommand, NotificationEntity>()
                .ForMember(entity => entity.Status, mapper => mapper.MapFrom(_ => NotificationStatus.Sent))
                .ForMember(entity => entity.CreatedAt, mapper => mapper.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
