using AutoMapper;
using MessagesService.Application.Messages.Commands.SendMessage;
using MessagesService.Core.Enums;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Entities;

namespace MessagesService.Application.Messages.Mapping
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<SaveMessageCommand, MessageEntity>()
                .ForMember(entity => entity.IsRead, mapper => mapper.MapFrom(_ => false))
                .ForMember(entity => entity.Type, mapper => mapper.MapFrom(_ => (int)MessageType.User))
                .ForMember(entity => entity.EditedAt, mapper => mapper.MapFrom(_ => (DateTime?)null))
                .ForMember(entity => entity.SentAt, mapper => mapper.MapFrom(_ => DateTime.UtcNow));

            CreateMap<MessageEntity, Message>()
                .ForMember(msg => msg.Type, mapper => mapper.MapFrom(entity => (MessageType)entity.Type));
        }
    }
}
