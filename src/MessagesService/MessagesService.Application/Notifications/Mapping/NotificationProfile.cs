using AutoMapper;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Entities;

namespace MessagesService.Application.Notifications.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationEntity, Notification>();
        }
    }
}
