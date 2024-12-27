using AutoMapper;
using MediatR;
using MessagesService.Core.Constants;
using MessagesService.Core.Enums;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Chats.Commands.AddChat
{
    public class AddChatCommandHandler : IRequestHandler<AddChatCommand, Chat>
    {
        private readonly ILogger<AddChatCommandHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IChatsRepository _chatsRepository;
        private readonly IMapper _mapper;

        public AddChatCommandHandler(
            ILogger<AddChatCommandHandler> logger,
            IMessagesRepository messagesRepository,
            IChatsRepository chatsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _chatsRepository = chatsRepository;
            _mapper = mapper;
        }

        public async Task<Chat> Handle(AddChatCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for user {UserId} and company {CompanyId}",
                request.GetType().Name,
                request.UserId,
                request.CompanyId);

            var chatEntity = _mapper.Map<ChatEntity>(request);
            var firstMessage = GetFirstMessage(chatEntity.CreatedAt);

            await _chatsRepository.AddAsync(chatEntity, token);

            firstMessage.ChatId = chatEntity.Id;

            await _messagesRepository.AddAsync(firstMessage, token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for chat {UserId} and company {CompanyId}",
                request.GetType().Name,
                request.UserId,
                request.CompanyId);

            return _mapper.Map<Chat>(chatEntity);
        }

        public MessageEntity GetFirstMessage(DateTime creationTime)
        {
            return new MessageEntity()
            {
                Content = MessagesTemplates.FirstApplicationResponse,
                SentAt = creationTime,
                Type = (int)MessageType.Creation,
            };
        }
    }
}
