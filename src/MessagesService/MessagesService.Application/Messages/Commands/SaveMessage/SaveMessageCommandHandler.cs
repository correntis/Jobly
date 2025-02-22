using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Commands.SendMessage
{
    public class SaveMessageCommandHandler : IRequestHandler<SaveMessageCommand, Message>
    {
        private readonly ILogger<SaveMessageCommandHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IChatsRepository _chatsRepository;
        private readonly IMapper _mapper;

        public SaveMessageCommandHandler(
            ILogger<SaveMessageCommandHandler> logger,
            IMessagesRepository messagesRepository,
            IChatsRepository chatsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _chatsRepository = chatsRepository;
            _mapper = mapper;
        }

        public async Task<Message> Handle(SaveMessageCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for chat {ChatId} and sender {SenderId}",
                request.GetType().Name,
                request.ChatId,
                request.SenderId);

            var chatEntity = await _chatsRepository.GetOneByAsync(chat => chat.Id, request.ChatId, token);

            var messageEntity = _mapper.Map<MessageEntity>(request);

            await _messagesRepository.AddAsync(messageEntity, token);

            await _chatsRepository.SetByIdAsync(chatEntity.Id, chat => chat.LastMessageAt, messageEntity.SentAt);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for chat {ChatId} and sender {SenderId}",
                request.GetType().Name,
                request.ChatId,
                request.SenderId);

            return _mapper.Map<Message>(messageEntity);
        }
    }
}
