using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Commands.EditMessage
{
    public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Message>
    {
        private readonly ILogger<EditMessageCommandHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IChatsRepository _chatsRepository;
        private readonly IMapper _mapper;

        public EditMessageCommandHandler(
            ILogger<EditMessageCommandHandler> logger,
            IMessagesRepository messagesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        public async Task<Message> Handle(EditMessageCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for message {MessageId}",
                request.GetType().Name,
                request.MessageId);

            var messageEntity = await _messagesRepository.GetOneBy(msg => msg.Id, request.MessageId, token);

            await _messagesRepository.SetByIdAsync(
                request.MessageId,
                msg => msg.Content,
                request.Content,
                token);

            messageEntity.Content = request.Content;

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for message {MessageId}",
                request.GetType().Name,
                request.MessageId);

            return _mapper.Map<Message>(messageEntity);
        }
    }
}
