using AutoMapper;
using MessagesService.Application.Chats.Commands.AddChat;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Entities;

namespace MessagesService.Application.Chats.Mapping
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<AddChatCommand, ChatEntity>()
                .ForMember(entity => entity.CreatedAt, mapper => mapper.MapFrom(_ => DateTime.UtcNow))
                .ForMember(entity => entity.LastMessageAt, mapper => mapper.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ChatEntity, Chat>();
        }
    }
}
