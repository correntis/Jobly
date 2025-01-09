using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Commands.ReadMessage
{
    public class ReadMessageCommandHandler : IRequestHandler<ReadMessageCommand, Message>
    {
        private readonly ILogger<ReadMessageCommandHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public ReadMessageCommandHandler(
            ILogger<ReadMessageCommandHandler> logger,
            IMessagesRepository messagesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        public async Task<Message> Handle(ReadMessageCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for message {MessageId}",
                request.GetType().Name,
                request.MessageId);

            var messageEntity = await _messagesRepository.GetOneBy(msg => msg.Id, request.MessageId, token);

            await _messagesRepository.SetByIdAsync(
                request.MessageId,
                msg => msg.IsRead,
                true,
                token);

            messageEntity.IsRead = true;

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for message {MessageId}",
                request.GetType().Name,
                request.MessageId);

            return _mapper.Map<Message>(messageEntity);
        }
    }
}
