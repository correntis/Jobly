using AutoMapper;
using MessagesService.Application.Messages.Commands.SendMessage;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Entities;

namespace MessagesService.Application.Messages.Mapping
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<SendMessageCommand, MessageEntity>()
                .ForMember(command => command.IsRead, mapper => mapper.MapFrom(_ => false))
                .ForMember(command => command.EditedAt, mapper => mapper.MapFrom(_ => (DateTime?)null))
                .ForMember(command => command.SentAt, mapper => mapper.MapFrom(_ => DateTime.UtcNow));

            CreateMap<MessageEntity, Message>();
        }
    }
}
